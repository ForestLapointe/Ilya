using System.Collections.Generic;
using UnityEngine;


public interface Combat
{
    #region Variables
    float health { get; set; }
    int team { get; set; }
    Transform transform { get; }
    Attack CurrentAttack { get; set; }
    List<Effect> effects { get; set; }
    #endregion Variables

    void addeffect (Effect effect);

    #region Classes
    [System.Serializable] public class Effect
    {
        public float tickrate;
        public float time;
        public float currenttime;
        public float potency;
        public Type type;
        public enum Type
        {
            damageovertime, cripple, powermodifier, defensemodifier, speedmodifier
        } 
    }
    #endregion Classes

    static bool findnearest (Vector3 position, float range, int excludeTeam, out Combat entity)
    {
        float distto = range;
        entity = null;
        for (int i = 0; i < Combat.Teams.Count; i++)
        {
            if (i == excludeTeam) continue;
            foreach (Combat member in Combat.Teams[i])
            {
                float distance = (position - member.transform.position).magnitude;
                if (distance < distto && member.health > 0)
                {
                    distto = distance;
                    entity = member;
                }
            }
        }
        return entity != null;
    }
    static List<List<Combat>> Teams;

    static void AddMember(Combat member)
    {
        for (int i = Teams.Count; i <= member.team; i++)
        {
            Teams.Add(new List<Combat>());
        }
        Teams[member.team].Add(member);
    }
}
