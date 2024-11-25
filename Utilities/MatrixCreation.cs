using Terraria;

namespace a3DLib.Utilities
{
    public static class MatrixCreation
    {
        /// <summary>
        /// Returns a world matrix that describes the position, rotation and scale of this object inside the game world.
        /// </summary>
        /// <param name="scales">A Vector3 that represents the X, Y and Z scales of this model.</param>
        /// <param name="rotations">A Vector3 that represents the X, Y and Z rotations of this model.</param>
        /// <param name="worldPosition">A Vector2 that represents the X and Y positions of this object in the game world.</param>
        /// <param name="zPos">The Z position of this model inside the game world. For larger models, this needs to be adjusted along with camera Zs and orthographic planes to avoid clipping.</param>
        /// <returns></returns>
        public static Matrix World(Vector3 scales, Vector3 rotations, Vector2 worldPosition, float zPos = 0f)
        {
            return Matrix.CreateScale(scales) * Matrix.CreateRotationX(rotations.X) * Matrix.CreateRotationY(rotations.Y) * Matrix.CreateRotationZ(rotations.Z) * Matrix.CreateTranslation(worldPosition.ToVector3() + new Vector3(0f, 0f, zPos));
        }
        /// <inheritdoc cref="World(Vector3, Vector3, Vector2, float)"/>
        /// <param name="scale">The X, Y and Z scale of this model.</param>
        /// <param name="rotation">The Z rotation of this model (replicating rotation in 2D)</param>
        public static Matrix World(float scale, float rotation, Vector2 worldPosition, float zPos = 0f) => World(new Vector3(scale), new Vector3(0f, 0f, rotation), worldPosition, zPos);
        /// <inheritdoc cref="World(Vector3, Vector3, Vector2, float)"/>
        /// <param name="scale">The X, Y and Z scale of this model.</param>
        /// <param name="xRotation">The X rotation of this model (yaw)</param>
        /// <param name="yRotation">The Y rotation of this model (pitch)</param>
        /// <param name="zRotation">The Z rotation of this model (roll, replicates rotation in 2D)</param>
        public static Matrix World(float scale, float xRotation, float yRotation, float zRotation, Vector2 worldPosition, float zPos = 0f) => World(new Vector3(scale), new Vector3(xRotation, yRotation, zRotation), worldPosition, zPos);
        /// <inheritdoc cref="World(Vector3, Vector3, Vector2, float)"/>
        /// <param name="quaternion">A <see cref="Quaternion"/> that represents the rotation of this model.</param>
        public static Matrix World(Vector3 scales, Quaternion quaternion, Vector2 worldPosition, float zPos = 0f)
        {
            return Matrix.CreateScale(scales) * Matrix.CreateFromQuaternion(quaternion) * Matrix.CreateTranslation(worldPosition.ToVector3() + new Vector3(0f, 0f, zPos));
        }
        /// <inheritdoc cref="World(float, float, Vector2, float"/>
        /// <param name="quaternion">A <see cref="Quaternion"/> that represents the rotation of this model.</param>
        public static Matrix World(float scale, Quaternion quaternion, Vector2 worldPosition, float zPos = 0f) => World(new Vector3(scale), quaternion, worldPosition, zPos);
        /// <summary>
        /// Returns a view matrix that describes the position of the game camera, specifically for use with <see cref="CommonOrthographicProjection(float, float)"/>
        /// </summary>
        /// <param name="cameraZ">The Z position of the camera, which is centered on the game's actual camera</param>
        /// <param name="focusZ">The Z position of the camera's focus, which is centered on the game's actual camera</param>
        /// <returns></returns>
        public static Matrix CommonCameraView(float cameraZ = -10f, float focusZ = 0f)
        {
            Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.5f);

            Vector3 camera = new(screenCenter.X, screenCenter.Y, cameraZ);
            Vector3 focus = new(screenCenter.X, screenCenter.Y, focusZ);

            // the up vector is down because of how world coordinates work. otherwise the final model is drawn mirrored about the center of the screen
            return Matrix.CreateLookAt(camera, focus, -Vector3.Up);
        }
        /// <summary>
        /// Returns a matrix that matches the orthographic projection of the game. This may be used in most contexts.
        /// Automatically accounts for zoom.
        /// </summary>
        /// <param name="zNearPlane">The Z position of the near plane for this matrix, essentially, how far away the "eyes" are</param>
        /// <param name="zFarPlane">The Z position of the far plane for this matrix, essentially, how far the "eyes" can see</param>
        /// <returns></returns>
        public static Matrix CommonOrthographicProjection(float zNearPlane = -10f, float zFarPlane = 10f)
        {
            float aspectRatio = Main.screenWidth / (float)Main.screenHeight;
            float halfHeight = (Main.screenHeight * 0.5f) / Main.GameViewMatrix.Zoom.X;
            float halfWidth = halfHeight * aspectRatio;

            return Matrix.CreateOrthographicOffCenter(-halfWidth, halfWidth, -halfHeight, halfHeight, zNearPlane, zFarPlane);
        }
        /// <summary>
        /// Returns a matrix that matches ortographic projection for displaying models on the UI.
        /// Automatically accounts for UI scale.
        /// </summary>
        /// <param name="zNearPlane"></param>
        /// <param name="zFarPlane"></param>
        /// <returns></returns>
        public static Matrix UIProjection(float zNearPlane = -10f, float zFarPlane = 10f)
        {
            return Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, zNearPlane, zFarPlane);
        }
    }
}
