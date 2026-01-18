using UnityEngine;

public interface IHackable
{
    string HackPromptText { get; }
    bool CanHack { get; }
    void Hack();
}
