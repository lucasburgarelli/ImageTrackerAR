using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracker : MonoBehaviour
{
    private ARTrackedImageManager _trackedImageManager;

    public GameObject[] ARPrefabs;

    private Dictionary<string, GameObject> _instantiatedPrefabs = new();

    private void Awake()
    {
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        _trackedImageManager.trackedImagesChanged += OnTrackImagesChanged;
    }
    private void OnDisable()
    {
        _trackedImageManager.trackedImagesChanged -= OnTrackImagesChanged;
    }

    private void OnTrackImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        args.added.ForEach(trackedAddedImage =>
        {
            var imageName = trackedAddedImage.referenceImage.name;

            var matchPrefab = ARPrefabs.FirstOrDefault(prefab => string.Compare(prefab.name, imageName, System.StringComparison.OrdinalIgnoreCase) == 0);

            if (matchPrefab != null && !_instantiatedPrefabs.ContainsKey(imageName))
            {
                var newPrefab = Instantiate(matchPrefab, trackedAddedImage.transform);

                _instantiatedPrefabs[imageName] = newPrefab;
            }
        });

        args.updated.ForEach(trackedUpdateImage =>
        {
            _instantiatedPrefabs[trackedUpdateImage.referenceImage.name].SetActive(trackedUpdateImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking);
        });

        args.removed.ForEach(trackedRemovedmage =>
        {
            Destroy(_instantiatedPrefabs[trackedRemovedmage.referenceImage.name]);
            _instantiatedPrefabs.Remove(trackedRemovedmage.referenceImage.name);
        });
    }
}
