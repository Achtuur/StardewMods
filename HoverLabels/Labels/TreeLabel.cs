﻿using AchtuurCore.Framework.Borders;
using HoverLabels.Framework;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels.Labels;
internal class TreeLabel : BaseLabel
{
    Tree hoverTree;

    public TreeLabel(int? priority=null) : base(priority)
    {
    }

    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        return Game1.currentLocation.terrainFeatures.ContainsKey(cursorTile)
            && Game1.currentLocation.terrainFeatures[cursorTile] is Tree;
    }

    public override void SetCursorTile(Vector2 cursorTile)
    {
        base.SetCursorTile(cursorTile);
        this.hoverTree = Game1.currentLocation.terrainFeatures[cursorTile] as Tree;
    }
    public override void GenerateLabel()
    {
        string tree_name = GetTreeName(GetTreeType(hoverTree.treeType.Value)) ?? "Tree";
        AddBorder(new TitleLabel(tree_name));
        List<Label> description = new();
        // not fully grown
        if (hoverTree.growthStage.Value < 5)
        {
            int time = GetExpectedGrowTime(hoverTree);
            
            if (hoverTree.fertilized.Value)
            {
                // Only case where grow time is certain
                if (this.hoverTree.treeType.Value != Tree.mahoganyTree)
                    description.Add(new Label($"Fully grown in {time} days"));

                description.Add(new Label("Fertilized!"));
            } 
            else
            {
                // grow time is statistically determined (innaccurate)
                description.Add(new Label($"Expected to be fully grown within {time} days"));
            }
        }
        else if (hoverTree.stump.Value)
        {
            description.Add(new Label("Fully grown (stump)"));
        }
        else
        {
            description.Add(new Label("Fully grown!"));
        }

        AddBorder(description);
    }

    private int GetExpectedGrowTime(Tree tree)
    {
        int stagesLeft = 5 - hoverTree.growthStage.Value;
        // Mahogany trees are different for some fuckin reason
        if (tree.treeType.Value == Tree.mahoganyTree)
        {
            if (tree.fertilized.Value)
                return (int) (stagesLeft / 0.6f);
            return (int)(stagesLeft / 0.15f);
        }

        if (tree.fertilized.Value)
            return stagesLeft;

        return (int) (stagesLeft / 0.2f);
    }


    private string GetTreeName(int treeType)
    {
        switch (treeType)
        {
            case 1: return "Oak Tree";
            case 2: return "Maple Tree";
            case 3: return "Pine Tree";
            case 4: return "Winter Tree 1";
            case 5: return "Winter Tree 2";
            case 6: return "Palm Tree";
            case 7: return "Mushroom Tree";
            case 8: return "Mahogany Tree";
            case 9: return "Palm Tree"; // 2nd variation palm tree
        }
        return null;
    }

    private int GetTreeType(string treeTypeValue) 
    {
        if(int.TryParse(treeTypeValue, out int value))
        {
            return value;
        }
        return 0;
    }
}
