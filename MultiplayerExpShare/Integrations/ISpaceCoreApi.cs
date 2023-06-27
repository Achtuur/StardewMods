using StardewValley;

namespace MultiplayerExpShare.Integrations;

public interface ISpaceCoreApi
{
    string[] GetCustomSkills();
    int GetLevelForCustomSkill(Farmer farmer, string skill);
    void AddExperienceForCustomSkill(Farmer farmer, string skill, int amt);
    int GetProfessionId(string skill, string profession);
}
