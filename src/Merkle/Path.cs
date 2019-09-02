using System.Collections.Generic;
using System.Linq;
using System;

namespace Chromia.PostchainClient.GTV
{

    public class Path
    {
        public List<PathElement> PathElements;

        public Path(List<PathElement> pathElements)
        {
            this.PathElements = pathElements;
        }

        public PathElement GetCurrentPathElement()
        {
            return PathElements[0];
        }

        public int Size()
        {
            return PathElements.Count;
        }

        public Path Tail()
        {
            if(PathElements.Count == 0)
            {
                throw new System.Exception("Impossible to tail this array");
            }
            else
            {
                return new Path(PathElements.Skip(1).ToList());
            }
        }

        public string DebugString()
        {
            return "todo";
        }

        public override bool Equals(Object obj)
        {
            if((obj == null) || ! this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            Path p = (Path) obj;
            return this.PathElements.SequenceEqual(p.PathElements);
        }

        public override int GetHashCode()
        {
            return PathElements.GetHashCode();
        }

    }

    public class PathElement
    {
        public SearchablePathElement Previous;

        public PathElement(SearchablePathElement previous)
        {
            this.Previous = previous;
        }
    }

    public class PathLeafElement: PathElement
    {
        public PathLeafElement(SearchablePathElement previous): base(previous)
        {
        }

        public override bool Equals(Object obj)
        {
            if(this == obj)
            {
                return true;
            }
            if(this.GetType() != obj.GetType())
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

    }

    public abstract class SearchablePathElement: PathElement
    {
        public SearchablePathElement(SearchablePathElement previous): base(previous)
        {
        }

        public abstract dynamic GetSearchKey();
    }

    public class ArrayPathElement: SearchablePathElement
    {
        private int Index;
        public ArrayPathElement(SearchablePathElement previous, int index): base(previous)
        {
            this.Index = index;
        }

        public override dynamic GetSearchKey()
        {
            return Index;
        }

        public override bool Equals(object obj)
        {
            if(this == obj)
            {
                return true;
            }
            if(this.GetType() != obj.GetType())
            {
                return false;
            }
            ArrayPathElement p = (ArrayPathElement) obj;
            if(this.Index != p.Index)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return Index;
        }

    }

    public class DictPathElement: SearchablePathElement
    {
        private dynamic Key;
        public DictPathElement(SearchablePathElement previous, dynamic key): base(previous)
        {
            this.Key = key;
        }

        public override dynamic GetSearchKey()
        {
            return this.Key;
        }

        public override bool Equals(object obj)
        {
            if(this == obj)
            {
                return true;
            }
            if(this.GetType() != obj.GetType())
            {
                return false;
            }
            DictPathElement p = (DictPathElement) obj;
            if(this.Key != p.Key)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return Key;
        }
    }

    public class PathSet
    {
        private HashSet<Path> Paths;
        public PathSet(Path[] paths)
        {
            Paths = new HashSet<Path>(paths);
        }

        public bool IsEmpty()
        {
            return Paths.Count == 0;
        }

        public PathElement GetPathLeafOrElseAnyCurrentPathElement()
        {
            PathLeafElement leafElem = null;
            PathElement currElem = null;
            Tuple<Path, PathElement> prev = null;

            foreach(var path in Paths)
            {
                currElem = path.GetCurrentPathElement();
                if(currElem is PathLeafElement)
                {
                    leafElem = (PathLeafElement) currElem;
                }

                prev = ErrorCheckUnequalParent(path, currElem, prev.Item1, prev.Item2);
            }

            if(leafElem != null)
            {
                return leafElem;
            }
            else
            {
                return currElem; // It doesn't matter which one we return (Next step we will get the "previous" from this one)
            }

        }

        public Tuple<Path, PathElement> ErrorCheckUnequalParent(Path currPath, PathElement currElem, Path prevPath, PathElement prevElem)
        {
            if(prevElem != null)
            {
                if(currElem.Previous != prevElem.Previous)
                {
                    throw new Exception("Something is wrong, these paths do not have the same parent.");
                }
            }
            return new Tuple<Path, PathElement>(currPath, currElem);
        }

        public PathSet KeepOnlyArrayPaths()
        {
           return null;
        }

        public PathSet KeepOnlyDictPaths()
        {
           return null;
        }

        public PathSet GetTailIfFirstElementIsArrayOfThisIndexFromList(int index)
        {
           return null;
        }

        public PathSet GetTailIfFirstElementIsDictOfThisKeyFromList(dynamic key)
        {
           return null;
        }

        public PathSet GetTailFromList(dynamic searchKey, Func<dynamic, Path, Path> filterFun)
        {
            var retPaths = new List<Path>();

            foreach(var path in Paths)
            {
                var newPath = filterFun(searchKey, path);
                if(newPath != null)
                {
                    retPaths.Add(newPath);
                }
            }
            return new PathSet(retPaths.ToArray());
        }
    }

    public static class Util
    {
        public static void BuildPathFromArray(List<PathElement> arr)
        {
        
            
        }

        public static void GetTailIfFirstElementIsArrayOfThisIndex()
        {

        }

    }
}