
using System;
using UnityEngine;

public class GazeFilter
{

    const float DEFAULT_FREQUENCY = 30.0F;
    const float DEFAULT_MIN_CUT_OFF = 1.0F;
    const float DEFAULT_BETA = 0.007F;
    const float DEFAULT_D_CUT_OFF = 1.0F;

    private float freq;
    private float minCutOff;
    private float beta;
    private float dCutOff;

    private OneEuro filterX;
    private OneEuro filterY;

    private float filteredX;
    private float filteredY;

    public GazeFilter(float freq = DEFAULT_FREQUENCY, float minCutOff = DEFAULT_MIN_CUT_OFF, float beta = DEFAULT_BETA, float dCutOff = DEFAULT_D_CUT_OFF)
    {
        initFilter(freq, minCutOff, beta, dCutOff);
    }

    private void initFilter(float _freq, float _minCutOff, float _beta, float _dCutOff)
    {

        freq = _freq;
        minCutOff = _minCutOff;
        beta = _beta;
        dCutOff = _dCutOff;

        if (freq <= 0.0F)
        {
            freq = DEFAULT_FREQUENCY;
        }

        if (minCutOff <= 0.0F)
        {
            minCutOff = DEFAULT_MIN_CUT_OFF;
        }

        if (dCutOff <= 0.0F)
        {
            dCutOff = DEFAULT_D_CUT_OFF;
        }


        try
        {
            filterX = new OneEuro(freq, minCutOff, beta, dCutOff);
            filterY = new OneEuro(freq, minCutOff, beta, dCutOff);

            filteredX = 0;
            filteredY = 0;
        }
        catch (Exception e)
        {
            Debug.Log("filter init fail" + e);
        }

    }


    public bool filterValues(long timestamp, float x, float y)
    {

        if (isValidFloat(x) && isValidFloat(y))
        {
            try
            {

                filteredX = filterX.filter(x, timestamp);
                filteredY = filterY.filter(y, timestamp);


                if(!isValidFloat(filteredX) || !isValidFloat(filteredY))
                {
                    initFilter(freq, minCutOff, beta, dCutOff);
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        return false;
    }

    private bool isValidFloat(float value)
    {
        if (float.IsInfinity(value)) return false;
        if (float.IsNaN(value)) return false;
        return true;
    }

    public float getFilteredX()
    {
        return filteredX;
    }


    public float getFilteredY()
    {
        return filteredY;
    }


}
