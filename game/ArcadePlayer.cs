using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multitasking
{
    internal class ArcadePlayer : ArcadeGameObject
    {
        // Constant for speed of player
        private const int Speed = 8;

        // Constant for how often a projectile is shot
        private const double TimePerShot = 0.45;

        // Field Declaration
        private int windowWidth;
        private double timer;
        private bool isAlive;
        private Texture2D projectileTexture;
        private List<ArcadeProjectile> projectiles;
        private const int ProjectileXSize = 17;
        private const int ProjectileYSize = 33;
        bool godMode;

        /// <summary>
        /// public parameterized constructor that creates an Arcade Player object
        /// </summary>
        /// <param name="texture"></param> texture for the player object
        /// <param name="position"></param> position for the player object (used in other logic)
        /// <param name="windowWidth"></param> window width integer used for screen wrap logic
        public ArcadePlayer(Texture2D texture, Rectangle position, int windowWidth, Texture2D projectileTexture)
            : base(texture, position)
        {
            this.windowWidth = windowWidth;
            timer = TimePerShot;
            this.projectileTexture = projectileTexture;
            projectiles = new List<ArcadeProjectile>();
            isAlive = true;
        }

        /// <summary>
        /// get-only property for player position
        /// </summary>
        public Rectangle Position
        {
            get { return position; }
        }

        /// <summary>
        /// Getter and Setter property for whether
        /// the player is alive
        /// </summary>
        public bool IsAlive
        {
            get { return isAlive; }
            set { isAlive = value; }
        }
        /// <summary>
        /// get property for the list of projectiles
        /// </summary>
        public List<ArcadeProjectile> Projectiles
        {
            get { return projectiles; }
        }
        /// <summary>
        /// get/set property for GodMode checks in other
        /// methods
        /// </summary>
        public bool GodMode
        {
            get { return godMode; }
            set { godMode = value; }
        }
        /// <summary>
        /// Update method for player. Includes the logic necessary for movement/auto firing
        /// </summary>
        /// <param name="gameTime"> required GameTime object for update methods </param> 
        public override void Update(GameTime gameTime)
        {
            // Allows player to move left and right using
            // left and right arrow keys

            KeyboardState kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.Left))
            {
                position.X -= Speed;
                if (position.X < (windowWidth / 2) + 7)
                {
                    position.X = windowWidth - 265;
                }
            }

            if (kbState.IsKeyDown(Keys.Right))
            {
                position.X += Speed;
                if (position.X > windowWidth - 265)
                {
                    position.X = (windowWidth / 2) + 7;
                }
            }
            
            // Tracks time elapsed to know when to shoot another projectile
            timer -= gameTime.ElapsedGameTime.TotalSeconds;
            if(timer < 0)
            {
                ArcadeProjectile newProjectile = new ArcadeProjectile(projectileTexture, new Rectangle(position.X + texture.Width / 2, position.Y, ProjectileXSize, ProjectileYSize));
                newProjectile.PlayerProjectile = true;
                projectiles.Add(newProjectile);
                timer = TimePerShot;
            }
            
        }


    }
}
