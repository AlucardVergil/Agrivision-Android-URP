using UnityEngine;

public static class TextureProcessor
{
    private static float currentAngle;

    #region To Straighten Image And Crop It (Didn't use this after all)
    public static Texture2D StraightenAndCropTexture(Texture2D originalTexture, float tolerance = 0.9f, float blackTolerance = 0.1f)
    {
        // Step 1: Detect the tilt angle using empty spaces
        float detectedAngle = DetectTiltAngle(originalTexture, tolerance);

        // Step 2: Rotate the texture to straighten it
        Texture2D straightenedTexture = RotateTexture(originalTexture, -detectedAngle);

        // Step 3: Crop out remaining white spaces
        Texture2D croppedTexture = CropWhiteSpaces(straightenedTexture, tolerance, blackTolerance);

        return croppedTexture;
    }

    private static float DetectTiltAngle(Texture2D texture, float tolerance)
    {
        int width = texture.width;
        int height = texture.height;
        Color[] pixels = texture.GetPixels();

        // Initialize boundaries
        int topmostY = height, bottommostY = 0, leftmostX = width, rightmostX = 0;

        // Find boundaries of non-white pixels
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixel = pixels[y * width + x];

                // Check if pixel is not white/empty
                if (pixel.r < tolerance || pixel.g < tolerance || pixel.b < tolerance || pixel.a < tolerance)
                {
                    if (y < topmostY) topmostY = y;
                    if (y > bottommostY) bottommostY = y;
                    if (x < leftmostX) leftmostX = x;
                    if (x > rightmostX) rightmostX = x;
                }
            }
        }

        // Calculate angle based on the topmost and bottommost non-white pixels
        float deltaX = rightmostX - leftmostX;
        float deltaY = bottommostY - topmostY;

        float angle = Mathf.Atan2(deltaY, deltaX) * Mathf.Rad2Deg;

        Debug.Log($"Detected Tilt Angle: {angle} degrees");

        currentAngle = angle;

        return angle;
    }

    private static Texture2D RotateTexture(Texture2D texture, float angle)
    {
        int width = texture.width;
        int height = texture.height;
        Texture2D rotatedTexture = new Texture2D(width, height);

        float xCenter = width / 2f;
        float yCenter = height / 2f;
        float angleRad = Mathf.Deg2Rad * angle;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate new pixel position after rotation
                int srcX = Mathf.FloorToInt((x - xCenter) * Mathf.Cos(angleRad) - (y - yCenter) * Mathf.Sin(angleRad) + xCenter);
                int srcY = Mathf.FloorToInt((x - xCenter) * Mathf.Sin(angleRad) + (y - yCenter) * Mathf.Cos(angleRad) + yCenter);

                // Set the pixel if within bounds
                if (srcX >= 0 && srcX < width && srcY >= 0 && srcY < height)
                {
                    rotatedTexture.SetPixel(x, y, texture.GetPixel(srcX, srcY));
                }
                else
                {
                    rotatedTexture.SetPixel(x, y, Color.clear); // Transparent for out-of-bounds pixels
                }
            }
        }

        rotatedTexture.Apply();
        return rotatedTexture;
    }

    private static Texture2D CropWhiteSpaces(Texture2D texture, float tolerance = 0.9f, float blackTolerance = 0.1f)
    {
        // Get the pixel colors from the texture
        Color[] pixels = texture.GetPixels();
        int width = texture.width;
        int height = texture.height;

        // Define crop boundaries
        int minX = width, minY = height, maxX = 0, maxY = 0;

        // Find the boundaries of the non-white and non-black area
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixel = pixels[y * width + x];

                // Check if the pixel is not white (within tolerance) and not black (below blackTolerance)
                bool isNotWhite = pixel.r < tolerance || pixel.g < tolerance || pixel.b < tolerance || pixel.a < tolerance;
                bool isNotBlack = pixel.r > blackTolerance || pixel.g > blackTolerance || pixel.b > blackTolerance;

                if (isNotWhite && isNotBlack)
                {
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        // Calculate the new width and height
        int croppedWidth = maxX - minX + 1;
        int croppedHeight = maxY - minY + 1;

        // Get the cropped pixels
        Color[] croppedPixels = texture.GetPixels(minX, minY, croppedWidth, croppedHeight);

        // Create a new texture and apply the cropped pixels
        Texture2D croppedTexture = new Texture2D(croppedWidth, croppedHeight);
        croppedTexture.SetPixels(croppedPixels);
        croppedTexture.Apply();

        return croppedTexture;
    }
    #endregion



    public static Vector2[] AdjustUVsBasedOnRotation(Vector2[] uv)
    {
        // Convert angle to radians
        float radians = currentAngle * Mathf.Deg2Rad;

        // Create rotation matrix components
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        // Center point for rotation (typically the center of the texture)
        Vector2 center = new Vector2(0.5f, 0.5f);

        // Adjust each UV coordinate based on the rotation
        for (int i = 0; i < uv.Length; i++)
        {
            // Translate UV to origin
            Vector2 translated = uv[i] - center;

            // Apply rotation matrix
            float rotatedX = translated.x * cos - translated.y * sin;
            float rotatedY = translated.x * sin + translated.y * cos;

            // Translate UV back to original position
            uv[i] = new Vector2(rotatedX, rotatedY) + center;

            Debug.Log($"UV {i} = {uv[i]}");
        }

        return uv;
    }



    // Change the white pixels alpha value so that i can then use the material alpha clipping to delete them and leave just the colored texture
    public static Texture2D DeleteWhitePixels(Texture2D texture, float tolerance = 0.9f)
    {
        // Get the pixel colors from the texture
        Color[] pixels = texture.GetPixels();
        int width = texture.width;
        int height = texture.height;

        for (int i = 0; i < pixels.Length; i++)
        {
            Color pixel = pixels[i];

            // Check if the pixel is white (within tolerance)
            if (pixel.r >= tolerance && pixel.g >= tolerance && pixel.b >= tolerance)
            {
                // Set alpha to 0 to make it fully transparent
                pixels[i] = new Color(pixel.r, pixel.g, pixel.b, 0f);
            }
        }

        // Create a new texture and apply the modified pixels
        Texture2D newTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        newTexture.SetPixels(pixels);
        newTexture.Apply();

        return newTexture;
    }



}
