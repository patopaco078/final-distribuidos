using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class Bullet
    {
        public string Id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public Vector2 DirectionMovement { get; set; }
        public float Force { get; set; }
        public int Radius { get; set; }
        public bool isCollision { get; set; }

        public bool Collision(Player player)
        {
            if (!isCollision)
            {
                var dx = player.x - x;
                var dy = player.y - y;
                var rSum = Radius + player.Radius;

                return dx * dx + dy * dy <= rSum * rSum;
            }
            else
            { return false; }
        }
    }
}
