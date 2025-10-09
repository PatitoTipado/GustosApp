using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GustosApp.Infraestructure.ML
{
    public class OnnxInferenceService : IDisposable
    {
        private readonly InferenceSession _session;

        public OnnxInferenceService(string modelPath)
        {
            _session = new InferenceSession(modelPath);
        }

        public float[] Run(TokenizedInput input)
        {
            var seqLen = input.SequenceLength;

            var inputIdsTensor = TokenizerAdapter.ToDense1D(input.InputIds, seqLen);
            var tokenTypeTensor = TokenizerAdapter.ToDense1D(input.TokenTypeIds, seqLen);
            var attentionMaskTensor = TokenizerAdapter.ToDense1D(input.AttentionMask, seqLen);

            var onnxInputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input_ids", inputIdsTensor),
                NamedOnnxValue.CreateFromTensor("token_type_ids", tokenTypeTensor),
                NamedOnnxValue.CreateFromTensor("attention_mask", attentionMaskTensor)
            };

            using var results = _session.Run(onnxInputs);

            var first = results.First();
            var tensor = first.AsTensor<float>();
            var dims = tensor.Dimensions.ToArray();

            if (dims.Length == 3)
            {
                // [1, seq, hidden] -> promedio sobre seq
                int hidden = dims[2];
                int seq = dims[1];
                var arr = tensor.ToArray();
                var outVec = new float[hidden];
                for (int i = 0; i < seq; i++)
                    for (int j = 0; j < hidden; j++)
                        outVec[j] += arr[i * hidden + j];
                for (int j = 0; j < hidden; j++) outVec[j] /= seq;
                return outVec;
            }

            if (dims.Length == 2)
            {
                // [1, hidden]
                int hidden = dims[1];
                var outVec = new float[hidden];
                for (int j = 0; j < hidden; j++) outVec[j] = tensor[0, j];
                return outVec;
            }

            // fallback: aplanar
            return tensor.ToArray();
        }

        public void Dispose() => _session.Dispose();
    }
}
