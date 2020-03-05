using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public static class NodeNav
{


    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    public static Stack<T> TwinStarT<T>(ITraversable begNode, ITraversable endNode, bool dualSearch = true) where T : ITraversable
    {
        object chainLocker = new object();

        if(dualSearch)
        {
            Thread backwards = new Thread(() => SoloStar<T>(endNode, begNode, chainLocker, false, 0f, 1f));
            backwards.Start();
        }

        return SoloStar<T>(begNode, endNode, chainLocker);
    }



    public static Stack<T> SoloStar<T>(ITraversable begNode, ITraversable endNode, object chainLocker, bool canReturn = true, float hMod = 1f, float gMod = 1f) where T : ITraversable
    {
        if(begNode == endNode || begNode == null || endNode == null || !endNode.isTraversable) return null;


        List<ITraversable> openList = new List<ITraversable>();
        List<ITraversable> closedList = new List<ITraversable>();

        openList.Add(begNode);

        begNode.origin = null;
        ITraversable currentNode;

        while(openList.Count > 0)
        {
            currentNode = openList[0];

            foreach (ITraversable neighborNode in currentNode.GetConnectedTraversables())
            {
                if(neighborNode == null || neighborNode.isTraversable == false) { continue; }
                
                // Locks the chain modifying to prevent overriding
                lock(chainLocker)
                {
                    bool endInChain = neighborNode.CheckOriginChainFor(endNode);

                    if(endInChain)
                    {
                        if(canReturn == false)
                        {
                            return null;
                        }

                        neighborNode.ReverseOriginChain();
                        neighborNode.origin = currentNode;
                        Stack<T> returnStack = TraversableStackPath<T>(endNode);
                        
                        foreach(ITraversable tn in closedList)
                        {
                            tn.ClearOriginChain();
                        }
                        foreach(ITraversable tn in openList)
                        {
                            tn.ClearOriginChain();
                        }

                        return returnStack;
                    }

                    else
                    {
                        if(!closedList.Contains(neighborNode))
                        {
                            if(!openList.Contains(neighborNode))
                            {
                                neighborNode.origin = currentNode;
                                neighborNode.pathingValues[1] = neighborNode.GetTravelCostToRootOrigin() * gMod;
                                neighborNode.pathingValues[2] = (float)neighborNode.GetDistanceTo(endNode) * hMod;

                                AddToSortedList<ITraversable>(neighborNode, ref openList);
                            }
                        }

                        // We have already been to this node, so see if it's cheaper to the current node from here
                        else if(neighborNode.origin != currentNode &&
                            neighborNode.pathingValues[1] < currentNode.pathingValues[1])
                        {
                            currentNode.origin = neighborNode;
                        }
                    }
                }
            }

            closedList.Add(currentNode);
            openList.Remove(currentNode);
        }

        return null;
    }




    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private static Stack<T> TraversableStackPath<T>(ITraversable endNode) where T : ITraversable
    {
        Stack<T> returnStack = new Stack<T>();
        T currentNode = (T)endNode;

        currentNode.ValidateOriginChain();

        while(currentNode != null && !currentNode.origin.Equals(currentNode))
        {
            try
            {
                returnStack.Push(currentNode);
                currentNode = (T)currentNode.origin;
            }
            catch(System.OutOfMemoryException)
            {
                //UnityEngine.Debug.Log(returnStack.Count);
            }
        }

        return returnStack;
    }




    // ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ---------- ----------
    private static int AddToSortedList<T>(T node, ref List<T> sortedList) where T : System.IComparable<T>
    {
        for(int i = 0; i < sortedList.Count; i++)
        {
            if(node.CompareTo(sortedList[i]) < 0)
            {
                sortedList.Insert(i, node);
                return i;
            }
        }

        sortedList.Add(node);
        return sortedList.Count;
    }
}



public interface ITraversable : System.IComparable<ITraversable>
{
    ITraversable origin {get; set;}
    float[] coordinates {get; set;}
    float[] pathingValues {get; set;}

    bool isOccupied {get; set;}
    bool isTraversable {get; set;}


    ITraversable GetRootOrigin();
    ITraversable[] GetConnectedTraversables();

    void ClearOriginChain();
    void ReverseOriginChain();
    void ValidateOriginChain();
    bool CheckOriginChainFor(ITraversable higherOrigin);

    float GetTravelCostToRootOrigin();
    double GetDistanceTo(ITraversable destination);
}
