// Compatibility facade for older scripts and Inspector references.
public static class Quests
{
    public static bool VendorStarterQuest => Quest.VendorState != VendorQuestState.Completed;
    public static bool MercantStarterQuest => false;
}
