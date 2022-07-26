using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class InteractableTests
    {
        static readonly InteractableSelectMode[] s_SelectModes =
        {
            InteractableSelectMode.Single,
            InteractableSelectMode.Multiple,
        };

        static readonly XRBaseInteractable.DistanceCalculationMode[] s_DistanceCalculationMode =
        {
            XRBaseInteractable.DistanceCalculationMode.TransformPosition,
            XRBaseInteractable.DistanceCalculationMode.ColliderPosition,
            XRBaseInteractable.DistanceCalculationMode.ColliderVolume,
        };

        static readonly InteractorPositionOption[] s_InteractorPositionOption =
        {
            InteractorPositionOption.InteractorInsideCollider,
            InteractorPositionOption.InteractorOutsideCollider,
        };

        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllSceneObjects();
        }

        [UnityTest]
        public IEnumerator InteractableIsHoveredWhileAnyInteractorHovering()
        {
            TestUtilities.CreateInteractionManager();
            var interactor1 = TestUtilities.CreateMockInteractor();
            var interactor2 = TestUtilities.CreateMockInteractor();
            var interactable = TestUtilities.CreateGrabInteractable();

            Assert.That(interactable.isHovered, Is.False);
            Assert.That(interactable.interactorsHovering, Is.Empty);

            interactor1.validTargets.Add(interactable);
            interactor2.validTargets.Add(interactable);

            yield return null;

            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactable.interactorsHovering, Is.EquivalentTo(new[] { interactor1, interactor2 }));

            interactor2.validTargets.Clear();

            yield return null;

            Assert.That(interactable.isHovered, Is.True);
            Assert.That(interactable.interactorsHovering, Is.EquivalentTo(new[] { interactor1 }));

            interactor1.validTargets.Clear();

            yield return null;

            Assert.That(interactable.isHovered, Is.False);
            Assert.That(interactable.interactorsHovering, Is.Empty);
        }

        [UnityTest]
        public IEnumerator InteractableSelectModeSelect([ValueSource(nameof(s_SelectModes))] InteractableSelectMode selectMode)
        {
            TestUtilities.CreateInteractionManager();
            var interactor1 = TestUtilities.CreateMockInteractor();
            var interactor2 = TestUtilities.CreateMockInteractor();
            var interactable = TestUtilities.CreateGrabInteractable();
            interactable.selectMode = selectMode;

            interactor1.validTargets.Add(interactable);

            yield return null;

            Assert.That(interactable.isSelected, Is.True);
            Assert.That(interactable.interactorsSelecting, Is.EqualTo(new[] { interactor1 }));

            interactor2.validTargets.Add(interactable);

            yield return null;

            Assert.That(interactable.isSelected, Is.True);
            switch (selectMode)
            {
                case InteractableSelectMode.Single:
                    Assert.That(interactable.interactorsSelecting, Is.EqualTo(new[] { interactor2 }));
                    break;
                case InteractableSelectMode.Multiple:
                    Assert.That(interactable.interactorsSelecting, Is.EqualTo(new[] { interactor1, interactor2 }));
                    break;
                default:
                    Assert.Fail($"Unhandled {nameof(InteractableSelectMode)}={selectMode}");
                    break;
            }
        }

        [Test]
        public void InteractableDistanceCalculationModeWithInteractorInsideColliders([ValueSource(nameof(s_DistanceCalculationMode))]
            XRBaseInteractable.DistanceCalculationMode distanceCalculationMode)
        {
            TestUtilities.CreateInteractionManager();
            IXRInteractor interactor = TestUtilities.CreateMockInteractor();
            var interactable = TestUtilities.CreateSimpleInteractableWithColliders();

            interactor.transform.position = new Vector3(0.5f, 0f, 0f);
            interactable.distanceCalculationMode = distanceCalculationMode;

            Assert.That(interactable.distanceCalculationMode, Is.EqualTo(distanceCalculationMode));
            Assert.That(interactable.colliders.Count, Is.GreaterThan(1));

            var distanceSqr = interactable.GetDistanceSqrToInteractor(interactor);
            switch (distanceCalculationMode)
            {
                case XRBaseInteractable.DistanceCalculationMode.TransformPosition:
                    Assert.That(distanceSqr, Is.EqualTo(0.5f * 0.5f));
                    break;
                case XRBaseInteractable.DistanceCalculationMode.ColliderPosition:
                    Assert.That(distanceSqr, Is.EqualTo(0.5f * 0.5f));
                    break;
                case XRBaseInteractable.DistanceCalculationMode.ColliderVolume:
                    Assert.That(distanceSqr, Is.EqualTo(0f));
                    break;
                default:
                    Assert.Fail($"Unhandled {nameof(InteractableSelectMode)}={distanceCalculationMode}");
                    break;
            }
        }

        [Test]
        public void InteractableDistanceCalculationMode(
            [ValueSource(nameof(s_DistanceCalculationMode))] XRBaseInteractable.DistanceCalculationMode distanceCalculationMode,
            [ValueSource(nameof(s_InteractorPositionOption))] InteractorPositionOption interactorPositionOption)
        {
            TestUtilities.CreateInteractionManager();
            IXRInteractor interactor = TestUtilities.CreateMockInteractor();
            var interactable = TestUtilities.CreateSimpleInteractableWithColliders();

            // The sphere or box colliders have size of 1f and they are translated (below) to a random position inside a sphere of radius 0.25f
            var interactorPosition = interactorPositionOption == InteractorPositionOption.InteractorInsideCollider
                ? Random.insideUnitSphere * 0.2f
                : Random.onUnitSphere * 5f;

            interactor.transform.position = interactorPosition;
            interactable.distanceCalculationMode = distanceCalculationMode;

            Assert.That(interactable.distanceCalculationMode, Is.EqualTo(distanceCalculationMode));
            Assert.That(interactable.colliders.Count, Is.GreaterThan(1));

            // Move the colliders to random positions not far from the interactable
            foreach (var col in interactable.colliders)
                col.transform.position = Random.insideUnitSphere * 0.25f;

            var distanceSqr = interactable.GetDistanceSqrToInteractor(interactor);
            switch (distanceCalculationMode)
            {
                case XRBaseInteractable.DistanceCalculationMode.TransformPosition:
                    var offset = interactable.transform.position - interactorPosition;
                    Assert.That(distanceSqr, Is.EqualTo(offset.sqrMagnitude));
                    break;
                case XRBaseInteractable.DistanceCalculationMode.ColliderPosition:
                    var minColDistanceSqr = float.MaxValue;
                    foreach (var col in interactable.colliders)
                    {
                        offset = col.transform.position - interactorPosition;
                        minColDistanceSqr = Mathf.Min(minColDistanceSqr, offset.sqrMagnitude);
                    }
                    Assert.That(distanceSqr, Is.EqualTo(minColDistanceSqr));
                    break;
                case XRBaseInteractable.DistanceCalculationMode.ColliderVolume:
                    minColDistanceSqr = float.MaxValue;
                    foreach (var col in interactable.colliders)
                    {
                        offset = col.ClosestPoint(interactorPosition) - interactorPosition;
                        minColDistanceSqr = Mathf.Min(minColDistanceSqr, offset.sqrMagnitude);
                    }
                    Assert.That(distanceSqr, Is.EqualTo(minColDistanceSqr));
                    break;
                default:
                    Assert.Fail($"Unhandled {nameof(InteractableSelectMode)}={distanceCalculationMode}");
                    break;
            }
        }

        public enum InteractorPositionOption
        {
            InteractorInsideCollider,
            InteractorOutsideCollider,
        }
    }
}
