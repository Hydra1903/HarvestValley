using UnityEngine;

public class PlayerLookAtAnimal : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float rayDistance = 5f;
    public Color rayColor = Color.green; // màu hiển thị raycast

    private AnimalInfo currentAnimal;

    void Update()
    {
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = transform.forward;

        // Vẽ raycast trong Scene view
        Debug.DrawRay(rayOrigin, rayDirection * rayDistance, rayColor);

        Ray ray = new Ray(rayOrigin, rayDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // Kiểm tra collider là BoxCollider
            BoxCollider box = hit.collider as BoxCollider;
            if (box != null)
            {
                AnimalInfo animal = hit.collider.GetComponent<AnimalInfo>();

                if (animal != null && animal != currentAnimal)
                {
                    // Ẩn panel cũ nếu có
                    if (currentAnimal != null)
                        currentAnimal.HideInfo();

                    currentAnimal = animal;

                    // Hiển thị theo loại vật từ AnimalData
                    if (animal.data != null)
                    {
                        switch (animal.data.animalType) // enum AnimalType {None, Goat, Sheep}
                        {
                            case AnimalTypeed.Goat:
                            case AnimalTypeed.Sheep:
                                currentAnimal.ShowInfo();
                                break;
                            default:
                                currentAnimal.HideInfo();
                                break;
                        }
                    }
                }
            }
            else
            {
                // Nếu trúng collider nhưng không phải BoxCollider
                if (currentAnimal != null)
                {
                    currentAnimal.HideInfo();
                    currentAnimal = null;
                }
            }
        }
        else
        {
            // Không còn nhìn con vật nào
            if (currentAnimal != null)
            {
                currentAnimal.HideInfo();
                currentAnimal = null;
            }
        }
    }
    public AnimalInfo CurrentAnimal => currentAnimal;
}
