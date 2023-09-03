using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    public LayerMask defaultLayer;

    [Header("Concrete")]
    public LayerMask concreteLayer;
    public AudioClip StepSoundConcrete;
    public AudioClip StartJumpSoundConcrete;
    public AudioClip LandJumpSoundConcrete;

    [Header("Overpass")]
    public LayerMask overpassLayer;
    public AudioClip StepSoundOverpass;
    public AudioClip StartJumpSoundOverpass;
    public AudioClip LandJumpSoundOverpass;

    [Header("Terrain")]
    public LayerMask terrainLayer;
    public AudioClip StepSoundTerrain;
    public AudioClip StartJumpSoundTerrain;
    public AudioClip LandJumpSoundTerrain;

    [Header("Tile")]
    public LayerMask tileLayer;
    public AudioClip StepSoundTile;
    public AudioClip StartJumpSoundTile;
    public AudioClip LandJumpSoundTile;

    private AudioSource audioSourse;
    private Animator animator;

    void Start()
    {
        audioSourse = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    public void PlayStartJumpSound(string leg)
    {
        RaycastHit hit;
        Vector3 legPosition = Vector3.zero;

        if (animator)
        {
            legPosition = (leg == "Left") ? animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position
                                          : animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position;
        }

        if (Physics.Linecast(legPosition, legPosition + Vector3.down, out hit))
        {
            // Concrete
            if ((1 << hit.transform.gameObject.layer) == concreteLayer ||
                (1 << hit.transform.gameObject.layer) == defaultLayer)
            {
                if (StartJumpSoundConcrete)
                {
                    audioSourse.PlayOneShot(StartJumpSoundConcrete);
                }
            }

            // Overpass
            else if ((1 << hit.transform.gameObject.layer) == overpassLayer)
            { 
                if (StartJumpSoundOverpass)
                {
                    audioSourse.PlayOneShot(StartJumpSoundOverpass);
                }
            }

            // Terrain
            else if ((1 << hit.transform.gameObject.layer) == terrainLayer)
            {
                if (StartJumpSoundTerrain)
                {
                    audioSourse.PlayOneShot(StartJumpSoundTerrain);
                }
            }

            // Tile
            else if ((1 << hit.transform.gameObject.layer) == tileLayer)
            {
                if (StartJumpSoundTile)
                {
                    audioSourse.PlayOneShot(StartJumpSoundTile);
                }
            }
        }
    }

    public void PlayLandingSound(string leg)
    {
        RaycastHit hit;
        Vector3 legPosition = Vector3.zero;

        if (animator)
        {
            legPosition = (leg == "Left") ? animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position
                                          : animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position;
        }

        if (Physics.Linecast(legPosition, legPosition + Vector3.down, out hit))
        {
            // Concrete
            if ((1 << hit.transform.gameObject.layer) == concreteLayer ||
                (1 << hit.transform.gameObject.layer) == defaultLayer)
            {
                if (LandJumpSoundConcrete)
                {
                    audioSourse.PlayOneShot(LandJumpSoundConcrete);
                }
            }

            // Overpass
            else if ((1 << hit.transform.gameObject.layer) == overpassLayer)
            {
                if (LandJumpSoundOverpass)
                {
                    audioSourse.PlayOneShot(LandJumpSoundOverpass);
                }
            }

            // Terrain
            else if ((1 << hit.transform.gameObject.layer) == terrainLayer)
            {
                if (LandJumpSoundTerrain)
                {
                    audioSourse.PlayOneShot(LandJumpSoundTerrain);
                }
            }

            // Tile
            else if ((1 << hit.transform.gameObject.layer) == tileLayer)
            {
                if (LandJumpSoundTile)
                {
                    audioSourse.PlayOneShot(LandJumpSoundTile);
                }
            }
        }
    }

    public void PlayStepSound(string leg)
    {
        RaycastHit hit;
        Vector3 legPosition = Vector3.zero;

        if (animator)
        {
            legPosition = (leg == "Left") ? animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position
                                          : animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position;
        }

        if (Physics.Linecast(legPosition, legPosition + Vector3.down, out hit))
        {
            // Concrete
            if ((1 << hit.transform.gameObject.layer) == concreteLayer ||
                (1 << hit.transform.gameObject.layer) == defaultLayer)
            {
                if (StepSoundConcrete)
                {
                    audioSourse.PlayOneShot(StepSoundConcrete);
                }
            }

            // Overpass
            else if ((1 << hit.transform.gameObject.layer) == overpassLayer)
            {
                if (StepSoundOverpass)
                {
                    audioSourse.PlayOneShot(StepSoundOverpass);
                }
            }

            // Terrain
            else if ((1 << hit.transform.gameObject.layer) == terrainLayer)
            {
                if (StepSoundTerrain)
                {
                    audioSourse.PlayOneShot(StepSoundTerrain);
                }
            }

            // Tile
            else if ((1 << hit.transform.gameObject.layer) == tileLayer)
            {
                if (StepSoundTile)
                {
                    audioSourse.PlayOneShot(StepSoundTile);
                }
            }
        }
    }
}
