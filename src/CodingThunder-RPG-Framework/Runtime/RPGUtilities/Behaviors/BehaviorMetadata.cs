namespace CodingThunder.RPGUtilities.Behaviors
{
    public struct BehaviorMetadata
    {
        public Actor Source { get; set; }
        public Actor Actor { get; set; }
        
        /// <summary>
        /// Set how long the Behavior runs.
        /// 0 means single-frame.
        /// -1 means tick once.
        /// 1 means wait for one second.
        /// </summary>
        public float Dur {  get; set; }

        /// <summary>
        /// Set how long to wait between ticks.
        /// 0 means single-frame.
        /// -1 means wait a single frame.
        /// 1 means wait one second.
        /// </summary>
        public float TickWait { get; set; }

    }
}