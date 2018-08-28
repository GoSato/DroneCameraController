using UnityEngine;

/// <summary>
///     フレームレートに依存しない速度で、徐々に時間をかけて望む目標に向かって値を変更します。
/// </summary>
public static class SmoothDamp
{
    #region float

    /// <summary>
    ///     <para>smoothnessは 0~1 の範囲の減衰率</para>
    ///     <para>値が大きいほど a が b に近づく速度は遅くなります</para>
    ///     <para>0 の場合は a = b となり補間なしで移動します</para>
    ///     <para>1 の場合は a = a となり完全に静止します</para>
    /// </summary>
    public static float Pow(float a, float b, float smoothness, float deltaTime)
    {
        smoothness = Mathf.Clamp01(smoothness);
        return Mathf.Lerp(a, b, 1 - Mathf.Pow(smoothness, deltaTime));
    }

    /// <summary>
    ///     <para>lambdaは 0~∞ の範囲の加速度</para>
    ///     <para>値が大きいほど a が b に近づく速度は速くなります</para>
    ///     <para>0 の場合は a = a　となり完全に静止します</para>
    ///     <para>値を十分に大きくすれば、極限の収束により a ≓ b　となりますが、a = b を保証する場合はPowを使用してください</para>
    /// </summary>
    public static float Exp(float a, float b, float lambda, float deltaTime)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * deltaTime));
    }

    #endregion

    #region Vector3

    /// <summary>
    ///     <para>smoothnessは 0~1 の範囲の減衰率</para>
    ///     <para>値が大きいほど a が b に近づく速度は遅くなります</para>
    ///     <para>0 の場合は a = b となり補間なしで移動します</para>
    ///     <para>1 の場合は a = a となり完全に静止します</para>
    /// </summary>
    public static Vector3 Pow(Vector3 a, Vector3 b, float smoothness, float deltaTime)
    {
        smoothness = Mathf.Clamp01(smoothness);
        return Vector3.Lerp(a, b, 1 - Mathf.Pow(smoothness, deltaTime));
    }

    /// <summary>
    ///     <para>lambdaは 0~∞ の範囲の加速度</para>
    ///     <para>値が大きいほど a が b に近づく速度は速くなります</para>
    ///     <para>0 の場合は a = a　となり完全に静止します</para>
    ///     <para>値を十分に大きくすれば、極限の収束により a ≓ b　となりますが、a = b を保証する場合はPowを使用してください</para>
    /// </summary>
    public static Vector3 Exp(Vector3 a, Vector3 b, float lambda, float deltaTime)
    {
        return Vector3.Lerp(a, b, 1 - Mathf.Exp(-lambda * deltaTime));
    }

    #endregion

    #region Quaternion

    /// <summary>
    ///     <para>smoothnessは 0~1 の範囲の減衰率</para>
    ///     <para>値が大きいほど a が b に近づく速度は遅くなります</para>
    ///     <para>0 の場合は a = b となり補間なしで移動します</para>
    ///     <para>1 の場合は a = a となり完全に静止します</para>
    /// </summary>
    public static Quaternion Pow(Quaternion a, Quaternion b, float smoothness, float deltaTime)
    {
        smoothness = Mathf.Clamp01(smoothness);
        return Quaternion.Lerp(a, b, 1 - Mathf.Pow(smoothness, deltaTime));
    }

    /// <summary>
    ///     <para>lambdaは 0~∞ の範囲の加速度</para>
    ///     <para>値が大きいほど a が b に近づく速度は速くなります</para>
    ///     <para>0 の場合は a = a　となり完全に静止します</para>
    ///     <para>値を十分に大きくすれば、極限の収束により a ≓ b　となりますが、a = b を保証する場合はPowを使用してください</para>
    /// </summary>
    public static Quaternion Exp(Quaternion a, Quaternion b, float lambda, float deltaTime)
    {
        return Quaternion.Lerp(a, b, 1 - Mathf.Exp(-lambda * deltaTime));
    }

    #endregion
}