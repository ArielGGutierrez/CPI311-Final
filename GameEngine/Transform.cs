using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class Transform : Component, IUpdateable
    {
        /* Variables */
        private Vector3 localPosition;
        private Quaternion localRotation;
        private Vector3 localScale;
        private Matrix world;

        /* Properties */
        //--------------------
        public Vector3 Position
        {
            get
            {
                return World.Translation;
            }

            set
            {
                if (Parent == null) LocalPosition = value;
                else
                {
                    Matrix total = World;
                    total.Translation = value;
                    LocalPosition = (Matrix.Invert(Matrix.CreateScale(LocalScale) *
                                     Matrix.CreateFromQuaternion(LocalRotation)) *
                                     total *
                                     Matrix.Invert(Parent.World)).Translation;
                }
            }
        }
        public Quaternion Rotation
        {
            get
            {
                return Quaternion.CreateFromRotationMatrix(World);
            }

            set
            {
                if (Parent == null) LocalRotation = value;
                else
                {
                    Vector3 scale, pos;
                    Quaternion rot;
                    world.Decompose(out scale, out rot, out pos);
                    Matrix total = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(value) * Matrix.CreateTranslation(pos);
                    Vector3 s, t;
                    Quaternion r;
                    (total * Matrix.Invert(Matrix.CreateFromQuaternion(LocalRotation) *
                                           Matrix.CreateTranslation(LocalPosition) *
                                           Parent.world))
                                           .Decompose(out s, out r, out t);
                    LocalRotation = r;
                }
            }
        }
        public Vector3 Scale
        { 
            get
            {
                Vector3 scale, pos; Quaternion rot;
                world.Decompose(out scale, out rot, out pos);
                return scale;
            }

            set
            {
                if (Parent == null) LocalScale = value;

                else
                {
                    Vector3 scale, pos;
                    Quaternion rot;
                    world.Decompose(out scale, out rot, out pos);
                    Matrix total = Matrix.CreateScale(value) * Matrix.CreateFromQuaternion(rot) * Matrix.CreateTranslation(pos);
                    Vector3 s, t;
                    Quaternion r;
                    (total * Matrix.Invert(Matrix.CreateFromQuaternion(LocalRotation) * 
                             Matrix.CreateTranslation(LocalPosition) * 
                             Parent.world)).Decompose(out s, out r, out t);
                    LocalScale = s;
                }
            }
        }
        public Vector3 LocalPosition { get{return localPosition;} set{localPosition = value; UpdateWorld();} }
        public Quaternion LocalRotation { get{return localRotation;} set{localRotation = value; UpdateWorld();} }
        public Vector3 LocalScale { get{return localScale;} set{localScale = value; UpdateWorld();} }
        public Matrix World { get{return world;} }

        /* Directions */
        public Vector3 Forward { get{return world.Forward;} }
        public Vector3 Backward { get{return world.Backward;} }
        public Vector3 Left { get{return world.Left;} }
        public Vector3 Right { get{return world.Right;} }
        public Vector3 Up { get { return world.Up; } }
        public Vector3 Down { get { return world.Down; } }

        /* Lab 4 - C */
        private Transform parent;
        private List<Transform> Children { get; set; }
        public Transform Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                if (parent != null)
                    parent.Children.Remove(this);

                if (parent != null)
                    parent.Children.Add(this);
                UpdateWorld();
            }
        }
        //--------------------

        /* Methods */
        public Transform()
        {
            localPosition = Vector3.Zero;
            localRotation = Quaternion.Identity;
            localScale = Vector3.One;
            Children = new List<Transform>();
            UpdateWorld();
        }

        public void Update()
        {
            UpdateWorld();
        }

        private void UpdateWorld()
        {
            world = Matrix.CreateScale(localScale) * Matrix.CreateFromQuaternion(localRotation) * Matrix.CreateTranslation(localPosition);
            if (parent != null)
                world *= parent.World;
            foreach (Transform child in Children)
                child.UpdateWorld();
        }

        public void Rotate(Vector3 axis, float angle)
        {
            LocalRotation *= Quaternion.CreateFromAxisAngle(axis, angle);
        }


    }
}
