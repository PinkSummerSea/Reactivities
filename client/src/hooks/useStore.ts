import { useContext } from "react";
import { StoreContext } from "../lib/stores/store";

export function useStore(){
    return useContext(StoreContext)
}