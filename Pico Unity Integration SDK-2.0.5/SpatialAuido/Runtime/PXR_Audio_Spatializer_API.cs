//  Copyright © 2015-2022 Pico Technology Co., Ltd. All Rights Reserved.

using System;
using System.Runtime.InteropServices;

namespace PXR_Audio
{
    namespace Spatializer
    {
        public class Api
        {
#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
    private static string dllname = "__Internal";
#else
            private const string DLLNAME = "PicoSpatializer";
#endif

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_get_version")]
            public static extern string GetVersion(ref int major, ref int minor, ref int patch);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_create_context")]
            public static extern Result CreateContext(
                ref IntPtr ctx,
                RenderingMode mode,
                uint framesPerBuffer,
                uint sampleRate);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_initialize_context")]
            public static extern Result InitializeContext(IntPtr ctx);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_submit_mesh")]
            public static extern Result SubmitMesh(
                IntPtr ctx,
                float[] vertices,
                int verticesCount,
                int[] indices,
                int indicesCount,
                AcousticsMaterial material,
                ref int geometryId);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_submit_mesh_and_material_factor")]
            public static extern Result SubmitMeshAndMaterialFactor(
                IntPtr ctx,
                float[] vertices,
                int verticesCount,
                int[] indices,
                int indicesCount,
                float[] absorptionFactor,
                float scatteringFactor,
                float transmissionFactor,
                ref int geometryId);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_get_absorption_factor")]
            public static extern Result GetAbsorptionFactor(
                AcousticsMaterial material,
                float[] absorptionFactor);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_get_scattering_factor")]
            public static extern Result GetScatteringFactor(
                AcousticsMaterial material,
                ref float scatteringFactor);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_get_transmission_factor")]
            public static extern Result GetTransmissionFactor(
                AcousticsMaterial material,
                ref float transmissionFactor);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_commit_scene")]
            public static extern Result CommitScene(IntPtr ctx);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_add_source")]
            public static extern Result AddSource(
                IntPtr ctx,
                SourceMode sourceMode,
                float[] position,
                ref int sourceId,
                bool isAsync);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_add_source_with_orientation")]
            public static extern Result AddSourceWithOrientation(
                IntPtr ctx,
                SourceMode mode,
                float[] position,
                float[] front,
                float[] up,
                float radius,
                ref int sourceId,
                bool isAsync);
            
            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_add_source_with_config")]
            public static extern Result AddSourceWithConfig(
                IntPtr ctx,
                ref SourceConfig sourceConfig,
                ref int sourceId,
                bool isAsync);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_source_attenuation_mode")]
            public static extern Result SetSourceAttenuationMode(IntPtr ctx,
                int sourceId,
                SourceAttenuationMode mode,
                DistanceAttenuationCallback directDistanceAttenuationCallback,
                DistanceAttenuationCallback indirectDistanceAttenuationCallback);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_source_range")]
            public static extern Result SetSourceRange(IntPtr ctx, int sourceId, float rangeMin, float rangeMax);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_remove_source")]
            public static extern Result RemoveSource(IntPtr ctx, int sourceId);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_submit_source_buffer")]
            public static extern Result SubmitSourceBuffer(
                IntPtr ctx,
                int sourceId,
                float[] inputBufferPtr,
                uint numFrames);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_submit_ambisonic_channel_buffer")]
            public static extern Result SubmitAmbisonicChannelBuffer(
                IntPtr ctx,
                float[] ambisonicChannelBuffer,
                int order,
                int degree,
                AmbisonicNormalizationType normType,
                float gain);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_submit_interleaved_ambisonic_buffer")]
            public static extern Result SubmitInterleavedAmbisonicBuffer(
                IntPtr ctx,
                float[] ambisonicBuffer,
                int ambisonicOrder,
                AmbisonicNormalizationType normType,
                float gain);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_submit_matrix_input_buffer")]
            public static extern Result SubmitMatrixInputBuffer(
                IntPtr ctx,
                float[] inputBuffer,
                int inputChannelIndex);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_get_interleaved_binaural_buffer")]
            public static extern Result GetInterleavedBinauralBuffer(
                IntPtr ctx,
                float[] outputBufferPtr,
                uint numFrames,
                bool isAccumulative);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_get_planar_binaural_buffer")]
            public static extern Result GetPlanarBinauralBuffer(
                IntPtr ctx,
                float[][] outputBufferPtr,
                uint numFrames,
                bool isAccumulative);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_get_interleaved_loudspeakers_buffer")]
            public static extern Result GetInterleavedLoudspeakersBuffer(
                IntPtr ctx,
                float[] outputBufferPtr,
                uint numFrames);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_get_planar_loudspeakers_buffer")]
            public static extern Result GetPlanarLoudspeakersBuffer(
                IntPtr ctx,
                float[][] outputBufferPtr,
                uint numFrames);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_update_scene")]
            public static extern Result UpdateScene(IntPtr ctx);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_doppler_effect")]
            public static extern Result SetDopplerEffect(IntPtr ctx, int sourceId, bool on);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_playback_mode")]
            public static extern Result SetPlaybackMode(
                IntPtr ctx,
                PlaybackMode playbackMode);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_loudspeaker_array")]
            public static extern Result SetLoudspeakerArray(
                IntPtr ctx,
                float[] positions,
                int numLoudspeakers);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_mapping_matrix")]
            public static extern Result SetMappingMatrix(
                IntPtr ctx,
                float[] matrix,
                int numInputChannels,
                int numOutputChannels);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_listener_position")]
            public static extern Result SetListenerPosition(
                IntPtr ctx,
                float[] position);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_listener_orientation")]
            public static extern Result SetListenerOrientation(
                IntPtr ctx,
                float[] front,
                float[] up);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_listener_pose")]
            public static extern Result SetListenerPose(
                IntPtr ctx,
                float[] position,
                float[] front,
                float[] up);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_source_position")]
            public static extern Result SetSourcePosition(
                IntPtr ctx,
                int sourceId,
                float[] position);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_source_gain")]
            public static extern Result SetSourceGain(
                IntPtr ctx,
                int sourceId,
                float gain);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_source_size")]
            public static extern Result SetSourceSize(
                IntPtr ctx,
                int sourceId,
                float volumetricSize);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_update_source_mode")]
            public static extern Result UpdateSourceMode(
                IntPtr ctx,
                int sourceId,
                SourceMode mode);

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_destroy")]
            public static extern Result Destroy(IntPtr ctx);
        }
    }
}