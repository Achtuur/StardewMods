using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolSkill
{
    internal class ToolSkill : SpaceCore.Skills.Skill
    {
        public const string TravelSkillID = "Achtuur.Tooling";

        // professions

        // path 1 : manual labor

        // lvl 5 -> tool upgrades cheaper (Smither)
        // lvl 10 -> more resources from tool use (Expert)
        // lvl 10 -> enchantments more potent? (Powerful)

        // path 2 : automisation
        // lvl 5 -> machines x% faster
        // lvl 10 -> machines x% chance of more output 
        // lvl 10 -> use hoe on machine to make it tick 2x fast for a day (1 use per day)



        /// <summary>
        /// Lvl 5: Grants additional x% movespeed
        /// </summary>-
        public static ToolSkillProfession ProfessionMovespeed;


        /// <summary>
        /// Lvl 10: Passively restore stamina
        /// </summary>
        public static ToolSkillProfession ProfessionRestoreStamina;


        /// <summary>
        /// Lvl 10: Walking in one direction for a set amount of time gives a speed boost, requires movespeed
        /// </summary>
        public static ToolSkillProfession ProfessionSprint;

        /// <summary>
        /// Lvl 5: Warp totem recipe is cheaper
        /// </summary>
        public static ToolSkillProfession ProfessionCheapWarpTotem;

        /// <summary>
        /// Lvl 10: Obelisk is cheaper, requires cheapwarptotem profession
        /// </summary>
        public static ToolSkillProfession ProfessionCheapObelisk;

        /// <summary>
        /// Lvl 10: Totems have a 50% chance of not being used up
        /// </summary>
        public static ToolSkillProfession ProfessionTotemReuse;

        public ToolSkill()
        : base(TravelSkillID) 
        {

            this.Icon = ModEntry.Instance.Helper.ModContent.Load<Texture2D>("assets/skillicon.png");
            this.SkillsPageIcon = ModEntry.Instance.Helper.ModContent.Load<Texture2D>("assets/skillpageicon.png");

            this.ExperienceCurve = new[] { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };
            this.ExperienceBarColor = new Microsoft.Xna.Framework.Color(100, 189, 132); // blueish green from skill icon

            // Level 5 professions

            ToolSkill.ProfessionMovespeed = new ToolSkillProfession(skill: this, id: "Movespeed", name: I18n.Movespeed_Name, desc: I18n.Movespeed_Desc, path_to_icon: "assets/professions/movespeed.png");
            this.Professions.Add(ToolSkill.ProfessionMovespeed);

            ToolSkill.ProfessionCheapWarpTotem = new ToolSkillProfession(skill: this, id: "CheapWarpTotem", name: I18n.Cheapwarptotem_Name, desc: I18n.Cheapwarptotem_Desc, path_to_icon: "assets/professions/cheaptotem.png");
            this.Professions.Add(ToolSkill.ProfessionCheapWarpTotem);

            this.ProfessionsForLevels.Add(new ProfessionPair(5, ToolSkill.ProfessionMovespeed, ToolSkill.ProfessionCheapWarpTotem));

            // Level 10 professions A
            ToolSkill.ProfessionRestoreStamina = new ToolSkillProfession(skill: this, id: "RestoreStamina", name: I18n.Restorestamine_Name, desc: I18n.Restorestamina_Desc, path_to_icon: "assets/professions/restorestamina.png");
            this.Professions.Add(ToolSkill.ProfessionRestoreStamina);

            ToolSkill.ProfessionSprint = new ToolSkillProfession(skill: this, id: "Sprint", name: I18n.Sprint_Name, desc: I18n.Sprint_Desc, path_to_icon: "assets/professions/sprint.png");
            this.Professions.Add(ToolSkill.ProfessionSprint);

            this.ProfessionsForLevels.Add(new ProfessionPair(10, ToolSkill.ProfessionRestoreStamina, ToolSkill.ProfessionSprint, ToolSkill.ProfessionMovespeed));

            // Level 10 professions B
            ToolSkill.ProfessionCheapObelisk = new ToolSkillProfession(skill: this, id: "CheapObelisk", name: I18n.Cheapobelisk_Name, desc: I18n.Cheapobelisk_Desc, path_to_icon: "assets/professions/cheapobelisk.png");
            this.Professions.Add(ToolSkill.ProfessionCheapObelisk);

            ToolSkill.ProfessionTotemReuse = new ToolSkillProfession(skill: this, id: "TotemReuse", name: I18n.Totemreuse_Name, desc: I18n.Totemreuse_Desc, path_to_icon: "assets/professions/totemreuse.png");
            this.Professions.Add(ToolSkill.ProfessionTotemReuse);

            this.ProfessionsForLevels.Add(new ProfessionPair(10, ToolSkill.ProfessionCheapObelisk, ToolSkill.ProfessionTotemReuse, ToolSkill.ProfessionCheapWarpTotem));
        }

        public override string GetName()
        {
            return I18n.Travelskill_Name();
        }

        public override List<string> GetExtraLevelUpInfo(int level)
        {
            return new()
            {
                I18n.Travelskill_LevelUpPerk(bonus: Math.Round(ModEntry.Instance.Config.LevelMovespeedBonus * 100.0f, 2))
            };
        }

        public override string GetSkillPageHoverText(int level)
        {
            return I18n.Travelskill_LevelUpPerk(bonus: Math.Round(100.0f * ModEntry.Instance.Config.LevelMovespeedBonus * level, 2));
        }
    }
}
