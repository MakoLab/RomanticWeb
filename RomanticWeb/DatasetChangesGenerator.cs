using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb
{
    internal class DatasetChangesGenerator
    {
        private IDictionary<EntityId,DeleteBehaviours> _markedForDeletion;
        private EntityQuadCollection _initialQuads;
        private EntityQuadCollection _currentQuads;
        private IDictionary<EntityId,DeleteBehaviours> _deletedEntities;
        private IDictionary<EntityId,IList<EntityQuad>> _quadsAdded=new Dictionary<EntityId,IList<EntityQuad>>();
        private IList<EntityQuad> _quadsRemoved=new List<EntityQuad>();
        private ISet<EntityId> _entitiesRemoved=new HashSet<EntityId>();
        private IDictionary<EntityId,ISet<EntityQuad>> _entitiesReconstructed=new Dictionary<EntityId,ISet<EntityQuad>>();

        internal DatasetChangesGenerator(EntityQuadCollection initialQuads,EntityQuadCollection currentQuads,IDictionary<EntityId,DeleteBehaviours> deletedEntities)
        {
            _initialQuads=initialQuads;
            _currentQuads=currentQuads;
            _deletedEntities=deletedEntities;
            _quadsAdded=new Dictionary<EntityId,IList<EntityQuad>>();
            _quadsRemoved=new List<EntityQuad>();
            _entitiesRemoved=new HashSet<EntityId>();
            _entitiesReconstructed=new Dictionary<EntityId,ISet<EntityQuad>>();
        }

        internal IDictionary<EntityId,DeleteBehaviours> MarkedForDeletion
        {
            get
            {
                if (_markedForDeletion==null)
                {
                    _markedForDeletion=new Dictionary<EntityId,DeleteBehaviours>();
                    foreach (KeyValuePair<EntityId,DeleteBehaviours> entityDeleted in _deletedEntities)
                    {
                        DeleteEntity(entityDeleted.Key,entityDeleted.Value);
                    }
                }

                return _markedForDeletion;
            }
        }

        internal DatasetChanges GenerateDatasetChanges()
        {
            _markedForDeletion=new Dictionary<EntityId,DeleteBehaviours>();
            foreach (KeyValuePair<EntityId,DeleteBehaviours> entityDeleted in _deletedEntities)
            {
                DeleteEntity(entityDeleted.Key,entityDeleted.Value);
            }

            foreach (EntityId entityId in _currentQuads)
            {
                if (!_markedForDeletion.ContainsKey(entityId))
                {
                    IEnumerable<EntityQuad> currentEntityQuads=_currentQuads[entityId];
                    IEnumerable<EntityQuad> initialEntityQuads=_initialQuads[entityId];
                    ProcessCurrentQuads(entityId,currentEntityQuads,initialEntityQuads);
                    ProcessInitialQuads(entityId,currentEntityQuads,initialEntityQuads);
                }
            }

            return new DatasetChanges(_quadsAdded.Values.SelectMany(item => item),_quadsRemoved,_entitiesReconstructed.Values.SelectMany(item => item),_entitiesRemoved);
        }

        private void DeleteEntity(EntityId entityId,DeleteBehaviours deleteBehaviour)
        {
            if (!_markedForDeletion.ContainsKey(entityId))
            {
                _markedForDeletion.Add(entityId,deleteBehaviour);
                if (!(entityId is BlankId))
                {
                    _entitiesRemoved.Add(entityId);
                }

                if ((deleteBehaviour&DeleteBehaviours.DeleteVolatileChildren)==DeleteBehaviours.DeleteVolatileChildren)
                {
                    foreach (EntityQuad quad in _currentQuads[entityId])
                    {
                        if ((quad.Predicate.Uri.AbsoluteUri!=Rdf.type.AbsoluteUri)&&
                            ((((deleteBehaviour&DeleteBehaviours.DeleteVolatileChildren)==DeleteBehaviours.DeleteVolatileChildren)&&(quad.Object.IsBlank))||
                            (((deleteBehaviour&DeleteBehaviours.DeleteChildren)==DeleteBehaviours.DeleteChildren)&&(!quad.Object.IsLiteral))))
                        {
                            DeleteEntity(quad.Object.ToEntityId(),deleteBehaviour);
                        }
                    }
                }
            }
        }

        private void ProcessCurrentQuads(EntityId entityId,IEnumerable<EntityQuad> currentEntityQuads,IEnumerable<EntityQuad> initialEntityQuads)
        {
            EntityQuad firstQuad=currentEntityQuads.FirstOrDefault();
            if (firstQuad!=null)
            {
                EntityId parentEntityId=(firstQuad.EntityId is BlankId?((BlankId)firstQuad.EntityId).RootEntityId:firstQuad.EntityId);
                foreach (EntityQuad quad in currentEntityQuads)
                {
                    if (parentEntityId==null)
                    {
                        AddUnique(_quadsAdded,quad);
                    }
                    else if (!initialEntityQuads.Contains(quad))
                    {
                        if (!_entitiesReconstructed.ContainsKey(parentEntityId))
                        {
                            AddUnique(_quadsAdded,quad);
                        }
                        else
                        {
                            AddUnique(_entitiesReconstructed[parentEntityId],quad);
                        }
                    }
                    else if ((!quad.Object.IsLiteral)&&(_markedForDeletion.ContainsKey(quad.Object.ToEntityId())))
                    {
                        DeleteQuad(parentEntityId,quad);
                    }
                }
            }
        }

        private void ProcessInitialQuads(EntityId entityId,IEnumerable<EntityQuad> currentEntityQuads,IEnumerable<EntityQuad> initialEntityQuads)
        {
            EntityQuad firstQuad=initialEntityQuads.FirstOrDefault();
            if (firstQuad!=null)
            {
                EntityId parentEntityId=(firstQuad.EntityId is BlankId?((BlankId)firstQuad.EntityId).RootEntityId:firstQuad.EntityId);
                foreach (EntityQuad quad in initialEntityQuads)
                {
                    if ((parentEntityId!=null)&&(!currentEntityQuads.Contains(quad)))
                    {
                        DeleteQuad(parentEntityId,quad);
                    }
                }
            }
        }

        private void DeleteQuad(EntityId parentEntityId,EntityQuad quad)
        {
            if (quad.Object.IsBlank)
            {
                if (!_entitiesReconstructed.ContainsKey(parentEntityId))
                {
                    ReconstructEntity(parentEntityId);
                }
            }
            else
            {
                AddUnique(_quadsRemoved,quad);
            }
        }

        private void ReconstructEntity(EntityId parentEntityId)
        {
            ISet<EntityQuad> reconstructedEntity=_entitiesReconstructed[parentEntityId]=new HashSet<EntityQuad>();
            IList<EntityId> keysToBeRemoved=new List<EntityId>();
            foreach (var item in _quadsAdded)
            {
                for (int index=0; index<_quadsAdded.Count; index++)
                {
                    EntityQuad currentQuad=item.Value[index];
                    EntityId localStrongEntityId=(currentQuad.EntityId is BlankId?((BlankId)currentQuad.EntityId).RootEntityId:currentQuad.EntityId);
                    if (localStrongEntityId==parentEntityId)
                    {
                        item.Value.RemoveAt(index);
                        index--;
                        reconstructedEntity.Add(currentQuad);
                    }
                }

                if (item.Value.Count==0)
                {
                    keysToBeRemoved.Add(item.Key);
                }
            }

            foreach (EntityId key in keysToBeRemoved)
            {
                _quadsAdded.Remove(key);
            }

            for (int index=0; index<_quadsRemoved.Count; index++)
            {
                EntityQuad currentQuad=_quadsRemoved[index];
                EntityId localStrongEntityId=(currentQuad.EntityId is BlankId?((BlankId)currentQuad.EntityId).RootEntityId:currentQuad.EntityId);
                if (localStrongEntityId==parentEntityId)
                {
                    _quadsRemoved.RemoveAt(index);
                    index--;
                }
            }

            foreach (EntityQuad quad in _currentQuads.GetEntityQuads(parentEntityId))
            {
                if ((quad.Object.IsLiteral)||((!_markedForDeletion.ContainsKey(quad.Object.ToEntityId()))&&
                    ((!_markedForDeletion.ContainsKey(quad.Subject.ToEntityId()))||
                    (((_markedForDeletion[quad.Subject.ToEntityId()]&DeleteBehaviours.NullifyVolatileChildren)==DeleteBehaviours.NullifyVolatileChildren)&&(!(quad.Subject.ToEntityId() is BlankId))))))
                {
                    AddUnique(reconstructedEntity,quad);
                }
            }
        }

        private void AddUnique(ICollection<EntityQuad> collection,EntityQuad item)
        {
            if (!collection.Contains(item))
            {
                collection.Add(item);
            }
        }

        private void AddUnique(IDictionary<EntityId,IList<EntityQuad>> dictionary,EntityQuad item)
        {
            IList<EntityQuad> list=null;
            if (!dictionary.TryGetValue(item.EntityId,out list))
            {
                dictionary[item.EntityId]=list=new List<EntityQuad>();
            }

            AddUnique(list,item);
        }
    }
}