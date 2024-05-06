using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// struct to store a found clue and its distance
public struct ClueDistance
{
    public Clue clue;
    public double distance;
}

public enum ClueFinderFilterOptions
{
    All,
    Found,
    Unfound
}

// Component to represent the list of clues in the game
public class ClueMap : MonoBehaviour
{
    // The magic number to convert degrees to meters at the equator
    double degreesToMetersFactor = 111139.0d;

    // List of clues
    public List<Clue> clues;

    // Utility method to get all found clues
    public List<Clue> GetFoundClues()
    {
        List<Clue> foundClues = new List<Clue>();
        foreach (Clue clue in clues)
        {
            if (clue.found)
            {
                foundClues.Add(clue);
            }
        }
        return foundClues;
    }

    // Utility method to get all unfound clues
    public List<Clue> GetUnfoundClues()
    {
        List<Clue> unfoundClues = new List<Clue>();
        foreach (Clue clue in clues)
        {
            if (!clue.found)
            {
                unfoundClues.Add(clue);
            }
        }
        return unfoundClues;
    }

    // Calculate the distance from the device's location to the clue
    double CalculateDistanceToClueInM(
        Vector3d devicePosition,
        Vector3d cluePosition
    )
    {
        Vector3d directionToClue = cluePosition - devicePosition;

        return System.Math.Sqrt(
            System.Math.Pow(
                directionToClue.x * degreesToMetersFactor *
                System.Math.Cos(devicePosition.x * Mathf.Deg2Rad),
                2
            ) +
            System.Math.Pow(
                directionToClue.y * degreesToMetersFactor,
                2
            )
        );
    }

    // Find the closest clue based on a location
    public ClueDistance FindClosestClue(
        Vector3d currentLocation,
        ClueFinderFilterOptions filter = ClueFinderFilterOptions.All
    )
    {
        // Filter the clues based on the foundFilter
        List<Clue> filteredClues = new List<Clue>();

        if (filter == ClueFinderFilterOptions.All)
        {
            filteredClues = this.clues;
        }
        else if (filter == ClueFinderFilterOptions.Found)
        {
            filteredClues = GetFoundClues();
        }
        else if (filter == ClueFinderFilterOptions.Unfound)
        {
            filteredClues = GetUnfoundClues();
        }

        // Compute distances to all clues and select the closest
        double closestDistance = double.MaxValue;
        int closestClueIndex = -1;
        for (int i = 0; i < filteredClues.Count; i++)
        {
            double distance = CalculateDistanceToClueInM(
                currentLocation,
                filteredClues[i].location
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestClueIndex = i;
            }
        }

        return new ClueDistance
        {
            clue = filteredClues[closestClueIndex],
            distance = closestDistance
        };
    }
}