namespace Characters
{
    public class PlayerLevelStats 
    {
        private string _levelName = "";
        private int _movesMade = 0;
        private int _timeTaken = 0;
        private int _deaths = 0;

        public string LevelName
        {
            get => _levelName;
            set => _levelName = value;
        }

        public int MovesMade
        {
            get => _movesMade;
            set => _movesMade = value;
        }

        public int TimeTaken
        {
            get => _timeTaken;
            set => _timeTaken = value;
        } // seconds?

        public int Deaths
        {
            get => _deaths;
            set => _deaths = value;
        }
    }
}
