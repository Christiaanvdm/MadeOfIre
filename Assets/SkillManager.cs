using System.Collections;
using System.Collections.Generic;

using UnityEngine;
namespace Complete
{

    public static class SkillTypes
    {
        public const string Damage = "double_damage";
        public const string Size = "double_size";
        public const string Blink = "blink";
        public const string Glacier = "spawn_glacier";
        public const string SplitShot = "split_shot";
        public const string Slow = "half_speed";
        public const string Duration = "double_duration";
        public const string Bounce = "bounce";
        public const string DodgeRoll = "dodge_roll";
        public const string Chain = "chain_shot";
    }

    public static class SkillTargets
    {
        public const string Ground = "ground";
        public const string Attack = "attack";
        public const string Skill = "skill";
    }

    [System.Serializable]
    public class PlayerState
    {
        public List<SkillDetail> current_deck = new List<SkillDetail>();

        public SkillDetail getRandomCard()
        {
            int value = Mathf.RoundToInt(Random.value * (current_deck.Count - 1f));
            return current_deck[value];
        }
    }

    public class SkillManager : MonoBehaviour
    {
        public AbstractSkillDetail info;
    }
}