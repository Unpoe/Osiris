using System;
using UnityEngine;

namespace Osiris
{
    public class EnumLabelArrayAttribute : PropertyAttribute
    {
        public string[] labels;

        public EnumLabelArrayAttribute(Type labelEnum, int inc_start, int inc_end) {
            labels = new string[inc_end - inc_start + 1];
            for (int i = inc_start; i <= inc_end; i++) {
                labels[i - inc_start] = Enum.GetName(labelEnum, i);
            }
        }
    }
}