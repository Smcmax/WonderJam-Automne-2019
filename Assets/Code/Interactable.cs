﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Interactable : MonoBehaviour
{
    private List<Player> interactors;

    public Collider2D interactionZone;
    public SpriteRenderer interactTooltip;

    public bool multipleInteraction = false;
    public bool canBeInteracted = true;
    public bool showAtAllTime = false;

    [Tooltip("How long to wait before being able to interact again")]
    [Range(0, 2.5f)]
    public float interactionCooldown;

    [Tooltip("What to do upon interaction")]
    public UnityEvent onInteraction;

    public void Awake()
    {
        interactors = new List<Player>();

        if (!interactionZone)
            Debug.Log("No interactionZone set for this object");
        else
            interactionZone.isTrigger = true;

        if (interactTooltip)
            interactTooltip.enabled = showAtAllTime;    
    }


    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            interactors.Add(other.gameObject.GetComponent<Player>());

            if (interactTooltip && canBeInteracted && !interactTooltip.enabled)
                interactTooltip.enabled = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.gameObject.GetComponent<Player>();
            interactors.Remove(player);

            if (interactTooltip && canBeInteracted && interactors.Count == 0 && !showAtAllTime)
                interactTooltip.enabled = false;
        }
    }

    public void Update()
    {
        if (Time.timeScale == 0f) return;

        if (canBeInteracted && interactors.Count > 0)
        {
            foreach (Player player in new List<Player>(interactors))
                if (player.m_rewiredPlayer.GetButtonDown("Interact"))
                {
                    if (onInteraction != null)
                    {
                        canBeInteracted = false;
                        interactTooltip.enabled = false;

                        onInteraction.Invoke();
                        StartCoroutine(ApplyInteractionCooldown());
                    }
                }
        }
    }


    private IEnumerator ApplyInteractionCooldown()
    {
        if (multipleInteraction)
            yield return new WaitForSeconds(interactionCooldown);
        else
            yield return null;

        canBeInteracted = true;
        interactTooltip.enabled = interactors.Count > 0;
    }
}