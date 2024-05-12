using System;
using System.Collections.Generic;
using System.Text;

namespace FinalProject
{
    public static class GameConstant
    {
        // Camera Constants
        public const float CameraHeight = 200f;
        public const float PlayfieldSizeX = 300;
        public const float PlayfieldSizeY = 300;

        // Player Constants
        public const float MaxMotherHealth = 8f;
        public const float MaxPlayerHealth = 4f;
        public const float MinPlayerSpeed = 100f;
        public const float MaxPlayerSpeed = 1000f;
        public const float MinRespawnTimer = 0.5f;
        public const float MaxRespawnTimer = 3f;
        public const float FireCooldown = 0.25f;
        public const float PlayerHitStun = 0.25f;
        public const float PlayerBoundingSphereScale = 5f;
        public const float MotherBoundingSphereScale = 30f;
        public const float EnemyDamage = 1f;

        // Projectile Constants
        public const int MaxProjectiles = 100;
        public const float BulletSpeed = 250f;
        public const float BulletBoundingSphereScale = 1f;

        // Enemy Constants
        public const int MaxEnemies = 10;
        public const float MinEnemySpawnTime = 0.125f;
        public const float MaxEnemySpawnTime = 3f;
        public const float MinEnemySpeed = 100f;
        public const float MaxEnemySpeed = 1000f;
        public const float EnemyBoundingSphereScale = 10f;

        // Point Constants
        public const int DamagePenalty = 2;
        public const int DeathPenalty = 50;
        public const int KillBonus = 25;

        public enum Weapons
        {
            Default,
            machine_gun,
            spread_gun
        }

        public enum Projectiles
        {
            bullet
        }

        public enum Enemies
        {
            asteroid,
            ship
        }

        public enum GameStates
        {
            Play,
            Lose
        }
    }
}
