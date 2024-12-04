using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Mechanics
{
    [CreateAssetMenu(fileName = "NewSpriteAnimSet", menuName = "CodingThunder/Animation/Sprite Animation Set")]
    public class SpriteAnimSetSO : ScriptableObject
    {
        [SerializeField]
        private List<SpriteAnim> spriteAnims;

        private Dictionary<string, SpriteAnim> spriteAnimDict;

        public SpriteAnim GetAnimByKey(string key)
        {
            if (!Application.isPlaying)
            {
                foreach(SpriteAnim spriteAnim in spriteAnims)
                {
                    if (spriteAnim.animName.ToLower() == key.ToLower())
                    {
                        return spriteAnim;
                    }
                }

                Debug.LogError($"Attempted to find animation key {key} in SpriteAnimSetSO {this.name}, but it doesn't exist.");
                return null;
            }

            if (spriteAnimDict == null)
            {
                spriteAnimDict = spriteAnims.ToDictionary(x => x.animName);
            }

            return spriteAnimDict[key];
        }
    }

    [Serializable]
    public class SpriteAnim
    {
        public string animName = "default";
        public List<Sprite> sprites;
    }
}