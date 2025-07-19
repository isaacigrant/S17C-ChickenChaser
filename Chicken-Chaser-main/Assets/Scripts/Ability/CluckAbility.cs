using AI;
using UnityEngine;
using ScriptableObjects;
using Utilities;

public class CluckAbility : AbstractAbility
{
    [SerializeField] private ParticleSystem _cluckParticle;
    [SerializeField] private AudioClip _cluckSound;

    private const float AUDIO_VOLUME = 0.3f;
    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponentInChildren<AudioSource>();
    }

    public override bool CanActivate()
    {
        return owner.GetCurrentSpeed() < 1.0f && base.CanActivate();
    }

    protected override void Activate()
    {
        _cluckParticle.Play();
        _source.pitch = Random.Range(0.8f, 1.2f);
        _source.PlayOneShot(_cluckSound, SettingsManager.currentSettings.SoundVolume * AUDIO_VOLUME);
        AudioDetection.onSoundPlayed.Invoke(transform.position, 10, 20, EAudioLayer.ChickenEmergency);
    }
    protected override int AbilityTriggerID()
    {
        return StaticUtilities.JumpAnimID;
    }
    protected override int AbilityBoolID()
    {
        return StaticUtilities.CluckAnimID;
    }
}
