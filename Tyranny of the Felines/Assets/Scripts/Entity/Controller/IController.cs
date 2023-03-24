using Matryoshka.Abilities;
using UnityEngine;

namespace Matryoshka.Entity.Controller
{
    public interface IController 
    {
        Vector2 GetMovement();

        Vector2 GetMouseDirection();

        AbilityType GetAbility();

        Vector2 GetMousePosition();

        //

        PlayerState GetPlayerState();

        void Create();
    }
}
