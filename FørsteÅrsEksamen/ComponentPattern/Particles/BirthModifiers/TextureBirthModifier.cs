using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.Particles.BirthModifiers
{
    public class TextureBirthModifier : BirthModifier
    {
        private readonly TextureNames[] _textures;
        private static Random random = new Random();

        public TextureBirthModifier(params TextureNames[] textures)
        {
            _textures = textures;
        }

        public override void Execute(Emitter e, GameObject go, IParticle p)
        {
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            TextureNames textureName = _textures[random.Next(_textures.Length)];
            sr.Sprite = GlobalTextures.Textures[textureName];
        }
    }
}
