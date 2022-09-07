﻿// This file was auto-generated by ML.NET Model Builder. 
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace SpotifyProgramForms
{
    public partial class MLModel1
    {
        /// <summary>
        /// model input class for MLModel1.
        /// </summary>
        #region model input class
        public class ModelInput
        {
            [LoadColumn(0), ColumnName(@"USER_ID")]
            public string USER_ID { get; set; }

            [LoadColumn(1), ColumnName(@"TRACK_ID")]
            public string TRACK_ID { get; set; }

            [LoadColumn(2), ColumnName(@"NAME")]
            public string NAME { get; set; }

            [LoadColumn(3), ColumnName(@"ARTIST")]
            public string ARTIST { get; set; }

            [LoadColumn(4), ColumnName(@"Acousticness")]
            public float Acousticness { get; set; }

            [LoadColumn(5), ColumnName(@"Danceability")]
            public float Danceability { get; set; }

            [LoadColumn(6), ColumnName(@"Duration_Ms")]
            public float Duration_Ms { get; set; }

            [LoadColumn(7), ColumnName(@"Energy")]
            public float Energy { get; set; }

            [LoadColumn(8), ColumnName(@"Instrumentalness")]
            public float Instrumentalness { get; set; }

            [LoadColumn(9), ColumnName(@"Key")]
            public float Key { get; set; }

            [LoadColumn(10), ColumnName(@"Liveness")]
            public float Liveness { get; set; }

            [LoadColumn(11), ColumnName(@"Loudness")]
            public float Loudness { get; set; }

            [LoadColumn(12), ColumnName(@"Mode")]
            public float Mode { get; set; }

            [LoadColumn(13), ColumnName(@"Speechiness")]
            public float Speechiness { get; set; }

            [LoadColumn(14), ColumnName(@"Tempo")]
            public float Tempo { get; set; }

            [LoadColumn(15), ColumnName(@"Time_Signature")]
            public float Time_Signature { get; set; }

            [LoadColumn(16), ColumnName(@"Valence")]
            public float Valence { get; set; }

            [LoadColumn(17), ColumnName(@"Like")]
            public float Like { get; set; }

        }

        #endregion

        /// <summary>
        /// model output class for MLModel1.
        /// </summary>
        #region model output class
        public class ModelOutput
        {
            [ColumnName("PredictedLabel")]
            public float Prediction { get; set; }

            [ColumnName("Score")]
            public float[] Score { get; set; }

        }

        #endregion

        private static string MLNetModelPath = Path.GetFullPath("MLModel1.zip");

        private static Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CreatePredictEngine(), true);

        /// <summary>
        /// Use this method to predict on <see cref="ModelInput"/>.
        /// </summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public static ModelOutput Predict(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            return predEngine.Predict(input);
        }

        public static void LoadRetrainModel()
        {
            PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CreatePredictEngine(), true);
        }

        private static PredictionEngine<ModelInput, ModelOutput> CreatePredictEngine()
        {
            var mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var _);
            return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }
    }
}