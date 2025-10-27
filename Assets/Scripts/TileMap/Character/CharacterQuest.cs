namespace HeroPotion
{
    public class CharacterQuest : TileMapCharacterCore
    {
        public override TileMapCharacterType CharacterType => TileMapCharacterType.Quest;
        public override GuildLocationEventType TargetType => GuildLocationEventType.Food;

        private const string QuestIconPath = "";
        private int _questId;

        public override void Initialize(string textureName)
        {
            base.Initialize(textureName);

            SetQuest();
        }

        private void SetQuest()
        {
            _questId = QuestManager.Instance.GetRandomQuestId();
            _emotion.SetIcon(QuestIconPath);
        }

        protected override void OnClickOrder()
        {
            QuestManager.Instance.OnQuestClick(_questId);
            //QuestData questData = QuestManager.Instance.GetQuestDataById(_questId);
            //SaveManager.Instance.MySaveData.exp += questData.reward;
            //SaveManager.Instance.SetSaveData(nameof(SaveData.exp), SaveManager.Instance.MySaveData.exp);
            TileMapManager.Instance.OnCharacterExit(this);
            base.OnClickOrder();
        }
    } 
}