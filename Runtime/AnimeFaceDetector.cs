using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using UnityEngine;
using Emgu;
using Emgu.CV;
using Emgu.CV.CvEnum;
using UnityEditor;

namespace Ulbe.Anime
{
    public class AnimeFaceDetector : MonoBehaviour
    {
        private string _lbpcascadePath = "";

        private const string XmlFileName = "lbpcascade_animeface.xml";

        public void Awake()
        {
            if (_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _Instance = this;
            name = GetType().Name;

            DontDestroyOnLoad(gameObject);

            try
            {
                Init();
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError($"{e.Message}",gameObject);
                Destroy(gameObject);
                return;
            }
        }

        public void Init()
        {
            if (IsInitialized)
                return;

            var xmlLocation = $@"{Application.persistentDataPath}/xml";
            _lbpcascadePath = $"{xmlLocation}/{XmlFileName}";

            if (File.Exists($"{_lbpcascadePath}"))
                return;

            // Find Cascade Classifier xml
            string path = AssetDatabase
                            .GetAllAssetPaths()
                            .FirstOrDefault(p => p.EndsWith(XmlFileName));

            if (string.IsNullOrEmpty(path))
                throw new FileNotFoundException($"File not found: {XmlFileName}");

            TextAsset xmlTextAsset = (TextAsset) AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));

            if (xmlTextAsset is null)
                throw new FileNotFoundException($"File not found: {path}");

            // Save file to persistentDataPath
            Directory.CreateDirectory(xmlLocation);
            File.WriteAllText($"{_lbpcascadePath}", xmlTextAsset.text);

            IsInitialized = true;
        }

        /// <summary>
        /// Check if an image contains an anime face.
        /// </summary>
        /// <param name="imagePath">Image URI</param>
        /// <param name="callback">Return <see langword="True"/> if an anime face is detected, otherwise return <see langword="False"/></param>
        public void IsAnimeImage(string imagePath, Action<bool> callback)
        {
            StartCoroutine(_IsAnimeImage(imagePath, $"{_lbpcascadePath}", callback));
        }

        /// <summary>
        /// Check if an texture contains an anime face.
        /// </summary>
        /// <param name="texutre">Texture2D</param>
        /// <param name="callback">Return <see langword="True"/> if an anime face is detected, otherwise return <see langword="False"/></param>
        public void IsAnimeImage(Texture2D texutre, Action<bool> callback)
        {
            byte[] image = texutre.EncodeToPNG();
            string filename = $"{Guid.NewGuid()}.png";
            string path = SaveImageToPersistentDataPath(image, filename);
            StartCoroutine(_IsAnimeImage(path, $"{_lbpcascadePath}", (IsAnime) =>
            {
                callback(IsAnime);
                if(File.Exists(path))
                    File.Delete(path);
            }));
        }

        private IEnumerator _IsAnimeImage(string file, string xml, Action<bool> callback)
        {
            bool isAnime = false;
            try
            {
                var cascade = new CascadeClassifier(xml);
                var image = CvInvoke.Imread(file, ImreadModes.Color);
                IOutputArray gray = new Mat();
                CvInvoke.CvtColor(image, gray, ColorConversion.Bgr2Gray);
                CvInvoke.EqualizeHist(gray, gray);
                Rectangle[] faces = cascade.DetectMultiScale(
                    gray,
                    scaleFactor: 1.1f,
                    minNeighbors: 5,
                    minSize: new Size(24, 24)
                );
                isAnime = faces.Length > 0;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message, gameObject);
            }
            callback(isAnime);
            yield break;
        }

        private static string SaveImageToPersistentDataPath(byte[] image, string fileName)
        {
            string path = $"{Application.persistentDataPath}/images/{fileName}";
            Directory.CreateDirectory($"{Application.persistentDataPath}/images");
            File.WriteAllBytes(path, image);
            return path;
        }

        public static AnimeFaceDetector Instance => _Instance ?? new GameObject().AddComponent<AnimeFaceDetector>();

        public bool IsInitialized { get; private set; }

        protected static AnimeFaceDetector _Instance;
    }
}

