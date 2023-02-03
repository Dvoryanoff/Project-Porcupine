using UnityEngine;

public class FurnitureActions : MonoBehaviour {

    // This file contains code which will likely be completely moved to
    // some LUA files later on and will be parsed at run-time.
    public static void Door_UpdateAction ( Furniture furn, float deltaTime ) {
        // Debug.Log ($"FurnitureActions {furn.furnParameters["openness"]}");
        if ( furn.GetParameter ( "is_openning" ) >= 1 ) {
            furn.ChangeParameter ( "openness", deltaTime * 4 ); // FIXME: Maybe a dooropenspeed parameter?
            if ( furn.GetParameter ( "openness" ) >= 1 ) {
                furn.SetParameter ( "is_openning", 0 );
            }
        } else {
            furn.ChangeParameter ( "openness", deltaTime * -4 );
        }
        furn.SetParameter ( "openness", Mathf.Clamp01 ( furn.GetParameter ( "openness" ) ) );

        if ( furn.cbOnChanged != null ) {
            furn.cbOnChanged ( furn );
        }
    }

    public static ENTERABILITY Door_IsEnterable ( Furniture furn ) {
        Debug.Log ( "Door_IsEnterable" );

        furn.SetParameter ( "is_openning", 1 );

        if ( furn.GetParameter ( "openness" ) >= 1 ) {
            return ENTERABILITY.Yes;
        }
        return ENTERABILITY.Soon;
    }
}

