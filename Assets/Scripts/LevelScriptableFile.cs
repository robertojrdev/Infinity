using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsFile", menuName = "Infinity Prototype/LevelScriptable", order = 0)]
public class LevelScriptableFile : ScriptableObject
{
    public List<LevelPackage> levels = new List<LevelPackage>();

    public bool GetNextLevel(Vector2Int current, ref Vector2Int next)
    {
        if(!HasLevel(current))
            return false;

        next = current;
        next.y++;
        if(next.y >= levels[current.x].Levels.Count) //if current package has no more level try to get the next
        {
            next.x++;
            next.y = 0;
            return HasLevel(next); //check if has the next package
        }
        
        return true;
    }

    public bool HasLevel(Vector2Int level)
    {
        var hasPackages = levels.Count > level.x;
        var hasLevels = false;
        if(hasPackages)
            hasLevels = levels[level.x].Levels.Count > level.y;

        return hasPackages && hasLevels;
    }

}

[System.Serializable]
public class LevelPackage
{
    [SerializeField] private List<Level> levels = new List<Level>();
    public List<Level> Levels { get => levels;}

    public void Add(Level level) => Levels.Add(level);
}