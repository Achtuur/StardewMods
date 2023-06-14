﻿using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolSkill
{
    internal class ToolSkillProfession : ToolSkill.Profession
    {
        private Func<string> m_professionName;
        private Func<string> m_professionDescription;


        public ToolSkillProfession(Skills.Skill skill, string id, Func<string> name, Func<string> desc, string path_to_icon) : base(skill, id)
        {
            this.Icon = ModEntry.Instance.Helper.ModContent.Load<Texture2D>(path_to_icon);
            this.m_professionName = name;
            this.m_professionDescription = desc;
        }

        public override string GetDescription()
        {
            return this.m_professionDescription();
        }

        public override string GetName()
        {
            return this.m_professionName();
        }

        public override void DoImmediateProfessionPerk()
        {
            base.DoImmediateProfessionPerk();
        }

        public override void UndoImmediateProfessionPerk()
        {
            base.UndoImmediateProfessionPerk();
        }
    }
}