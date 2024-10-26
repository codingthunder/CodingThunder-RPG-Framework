using NUnit.Framework;
using CodingThunder.RPGUtilities.Cmds;
using UnityEngine.TestTools;
using UnityEngine;

[TestFixture]
public class CmdExpressionTests
{
    [Test]
    public void ToCmd_WithValidExpression_ReturnsCorrectICmd()
    {
        var cmdExpression = new CmdExpression { expression = "Cmd=Brake:Target=Player" };
        var cmd = cmdExpression.ToCmd();

        Assert.IsNotNull(cmd, "Cmd should not be null for valid expressions.");
        Assert.AreEqual("Brake", cmd.Parameters["Cmd"]);
        Assert.AreEqual("Player", cmd.Parameters["Target"]);
    }

    [Test]
    public void ToCmd_MissingCmdKey_LogsErrorAndReturnsNull()
    {
        var cmdExpression = new CmdExpression { expression = "Target=Player" };

        LogAssert.Expect(LogType.Error, "Missing Cmd Key in CmdExpression: Target=Player");
        var cmd = cmdExpression.ToCmd();

        Assert.IsNull(cmd, "Cmd should be null if 'Cmd' key is missing.");
    }

    [Test]
    public void ToCmd_InvalidCmdType_LogsErrorAndReturnsNull()
    {
        var cmdExpression = new CmdExpression { expression = "Cmd=NonExistentCmd:Target=Player" };

        LogAssert.Expect(LogType.Error, "Bad Cmd name in CmdExpression: Cmd=NonExistentCmd:Target=Player");
        var cmd = cmdExpression.ToCmd();

        Assert.IsNull(cmd, "Cmd should be null if Cmd type is invalid.");
    }
}
