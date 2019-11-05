﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSUR_UI
{
    public class Parser
    {

        private static readonly byte origin = 4;

        private static string LanePosition(int pos)
        {
            int laneNum = (pos - origin) / 2;
            Debug.Log($"Lane position:{pos}");
            if ((pos & 1) == 0)
            {
                return laneNum >= 0 ? $"R{laneNum}" : $"L{-laneNum}";
            }
            else
            {
                if (laneNum == 0)
                {
                    return pos > origin ? "R0P" : "L0P";
                }
                else
                {
                    return laneNum > 0 ? $"R{laneNum}P" : $"L{-laneNum}P";
                }

            }
        }


        public static string ModuleNameFromNetInfo(string prefabName)
        {
            string savenameStripped = prefabName.Substring(prefabName.IndexOf('.') + 1);
            string moduleName = savenameStripped.Substring(0, savenameStripped.LastIndexOf('_'));
            return moduleName;
        }

        public static string GetBlocks(int bitmask, byte symmetry)
        {
            byte pointer = 0;
            byte counter = 0;
            string blocks = "";
            string blockName, leftBlockName = null;
            bool asymApplied = false;
            while (bitmask > 0)
            {
                while ((bitmask & 1) == 0)
                {
                    bitmask >>= 1;
                    pointer++;
                }
                while ((bitmask & 1) == 1)
                {
                    bitmask >>= 2;
                    pointer += 2;
                    counter++;
                }
                if (counter > 0)
                {
                    int ilane = pointer - 2;
                    blockName = LanePosition(ilane);
                    Debug.Log($"counter: {counter}, name: {blockName}");
                    // abbreviate nRn as nR
                    if (blockName.Substring(1) == counter.ToString())
                    {
                        blockName = blockName.Substring(0, 1);
                    }
                    if (symmetry == 255)
                    {
                        // first find if the road is centered, then should be nC
                        if (ilane - counter + 1 == origin)
                        {
                            blockName = counter.ToString() + "C";
                        }
                        else
                        {
                            // oneway road
                            blockName = counter.ToString() + blockName;
                        }
                    }
                    else if (blockName.Substring(1) == $"{counter - 1}P")
                    {
                        // nR(n-1)P is the one-way counterpart of nDC
                        if (symmetry == 0) blockName = $"{counter * 2}DC";
                        else if (symmetry == 1) blockName = $"{counter * 2 + 1}DS";
                        else if (symmetry == 2) blockName = $"{counter * 2 + 2}DS2";
                        else throw new ArgumentException("Asymmetric road should have at most difference of 2");
                    }
                    else if (symmetry == 0)
                    {
                        // two-way divided symmetric road
                        blockName = $"{counter * 2}D" + blockName;
                    }
                    else
                    {
                        // two-way divided asymmetric road, only apply to the innermost block
                        if (symmetry > 2)
                        {
                            throw new ArgumentException("Asymmetric road should have at most difference of 2");
                        }
                        // asymmetric roads should always be selected on the right
                        if (ilane - counter < origin)
                        {
                            throw new ArgumentException("Asymmetric road should be selected on the right side");
                        }
                        if (!asymApplied)
                        {
                            if (blockName == "R")
                            {
                                // the only asymmetry option of nR is (n+1)DC
                                if (symmetry == 1) blockName = $"{counter * 2 + 1}DC";
                                else throw new ArgumentException("Not enough lanes for asymmetric road");
                            }
                            else
                            {
                                string lanePos = blockName;
                                blockName = "-" + (counter + symmetry).ToString();
                                if (lanePos.Substring(1) == (counter + symmetry).ToString())
                                {
                                    blockName += lanePos.Substring(0, 1);
                                }
                                else
                                {
                                    blockName += lanePos;
                                }
                                leftBlockName = counter.ToString() + lanePos;
                            }
                        }
                        else
                        {
                            blockName = counter.ToString() + blockName;
                            leftBlockName = blockName;
                        }
                        asymApplied = true;
                    }
                    counter = 0;
                    if (leftBlockName != null)
                    {
                        blocks = leftBlockName + blocks + blockName;
                    }
                    else
                    {
                        blocks += blockName;
                    }
                }
            }
            return blocks;
        }


        public static string ModuleNameFromUI(int fromSelected, int toSelected, byte symmetry,
                                            bool uturnLane, bool hasSidewalk, bool hasBike)
        {
            // Empty selection or overlapping lanes always give no module
            if ((fromSelected == 0) || (toSelected == 0) || (fromSelected & fromSelected << 1) != 0 || (toSelected & toSelected << 1) != 0)
            {
                return null;
            }
            try
            {
                // calculate asset name prefix
                string key = "CSUR";
                if (fromSelected != toSelected)
                {
                    if ((fromSelected & toSelected) == fromSelected || (fromSelected & toSelected) == toSelected)
                        key += "-T";
                    else
                    {
                        int ratio = fromSelected > toSelected ? fromSelected / toSelected : toSelected / fromSelected;
                        if ((ratio & ratio - 1) == 0 && (ratio * fromSelected == toSelected || ratio * toSelected == fromSelected))
                            key += "-S";
                        else
                            key += "-R";
                    }
                }
                key += " " + GetBlocks(fromSelected, symmetry);
                if (fromSelected != toSelected)
                {
                    key += "=" + GetBlocks(toSelected, symmetry);
                }
                if (uturnLane) key += " uturn";
                if (!hasSidewalk) key += " express";
                Debug.Log($"Found module {key}");
                return key;
            }
            catch (ArgumentException e)
            {
                Debug.Log(e);
                return null;
            }
        }

    }
}