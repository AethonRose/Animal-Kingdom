using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Make static to be accessible without instantiating gameObject
public static class HelperUtilities
{

    //ValidateCheckEmptyString
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {

        //If stringToCheck is Empty output Debug.Log
        if (stringToCheck == "")
        {

            Debug.Log($"{fieldName} is empty and must contain a value in object {thisObject.name.ToString()}");
            //Yes Validation Error
            return true;
        }

        //No Validation Error
        return false;
    }

    //ValidateCheckEnumerableValues
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {

        //Default error = false & Set Count = 0
        bool error = false;
        int count = 0;

        //Loop through item/entry in the ListToCheck
        foreach (var item in enumerableObjectToCheck)
        {

            //If item in enumerable list is null, Set error = true
            if (item == null)
            {
                Debug.Log($"{fieldName} has null values in object {thisObject.name.ToString()}");
                error = true;
            }
            else
            {
                count++;
            }
        }

        //If count == 0, then there is no values in the Object, Set error = true
        if (count == 0)
        {
            Debug.Log($"{fieldName} has no values in object {thisObject.name.ToString()}");
            error = true;
        }

        //Return False if everything runs correctly, Return True if error occurs
        return error;
    }

}
