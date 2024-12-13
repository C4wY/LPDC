using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lopiege : MonoBehaviour
{
    public int dégâts = 1;
    public GameObject spikePrefab;
    public float spikeRiseDistance = 2f;
    public float spikeRiseDuration = 1f;
    public float spikeStayDuration = 1f;

    private HashSet<LPDC.Avatar> avatarsEnContact = new HashSet<LPDC.Avatar>();

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            var avatar = other.attachedRigidbody.GetComponent<LPDC.Avatar>();
            if (avatar != null && !avatarsEnContact.Contains(avatar))
            {
                avatar.Santé.FaireDégâts(dégâts);
                avatarsEnContact.Add(avatar);

                GameObject spike = Instantiate(spikePrefab, transform.position, Quaternion.Euler(-68.5f, -90f, 90f));
                StartCoroutine(AnimateSpike(spike));
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            var avatar = other.attachedRigidbody.GetComponent<LPDC.Avatar>();
            if (avatar != null && avatarsEnContact.Contains(avatar))
            {
                avatarsEnContact.Remove(avatar);
            }
        }
    }

    private IEnumerator AnimateSpike(GameObject spike)
    {
        Vector3 startPosition = spike.transform.position;
        Vector3 endPosition = startPosition + Vector3.up * spikeRiseDistance;
        float elapsedTime = 0f;

        // Animation montée
        while (elapsedTime < spikeRiseDuration)
        {
            float t = elapsedTime / spikeRiseDuration;
            spike.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spike.transform.position = endPosition;

        // Attente avant redescente
        yield return new WaitForSeconds(spikeStayDuration);

        // Animation descente
        elapsedTime = 0f;
        while (elapsedTime < spikeRiseDuration)
        {
            float t = elapsedTime / spikeRiseDuration;
            spike.transform.position = Vector3.Lerp(endPosition, startPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spike.transform.position = startPosition;

        Destroy(spike, 0.1f);
    }
}

