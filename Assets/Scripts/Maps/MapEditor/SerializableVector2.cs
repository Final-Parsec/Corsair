namespace FinalParsec.Corsair.Maps.MapEditor
{
    using System;
    using UnityEngine;

    /// <summary>
    ///     Class capable of storing information about a <see cref="Vector2" /> in a format which supports serialization.
    /// </summary>
    [Serializable]
    public class SerializableVector2
    {
        /// <summary>
        ///     X component of the <see cref="Vector2" />.
        /// </summary>
        public float x;

        /// <summary>
        ///     Y component of the <see cref="Vector2" />.
        /// </summary>
        public float y;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SerializableVector2" /> class based on an existing <see cref="Vector2" />.
        /// </summary>
        /// <param name="vector2Source">
        ///     The source <see cref="Vector2" /> to use when initializing the object.
        /// </param>
        public SerializableVector2(Vector2 vector2Source)
        {
            this.x = vector2Source.x;
            this.y = vector2Source.y;
        }

        /// <summary>
        ///     Exports a <see cref="Vector2" /> with information from this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="Vector2" /> with information from this instance.
        /// </returns>
        public Vector2 ToVector2()
        {
            return new Vector2(this.x, this.y);
        }
    }
}