using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NullGuard;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Provides an graph dependant constrain selector.</summary>
    public class ConditionalConstrainSelector : QueryComponent, ISelectableQueryComponent
    {
        private IList<IExpression> _expressions = null;

        /// <summary>Creates an instance of the conditional constrain selector.</summary>
        /// <param name="entityAccessor">Target entity accessor to wrap around.</param>
        /// <param name="entityConstrain">Entity constrain.</param>
        /// <param name="fallbackConstrain">Fallback constrain.</param>
        public ConditionalConstrainSelector(StrongEntityAccessor entityAccessor, EntityConstrain entityConstrain, EntityConstrain fallbackConstrain)
        {
            EntityAccessor = entityAccessor;
            EntityConstrain = entityConstrain;
            FallbackConstrain = fallbackConstrain;
        }

        /// <summary>Gets an enumeration of selectable expressions.</summary>
        IEnumerable<IExpression> ISelectableQueryComponent.Expressions
        {
            get
            {
                if (_expressions == null)
                {
                    _expressions = new List<IExpression>();
                    Call bound = new Call(MethodNames.Bound);
                    bound.Arguments.Add(EntityAccessor);

                    Call subject = new Call(MethodNames.If);
                    subject.Arguments.Add(bound);
                    subject.Arguments.Add(EntityConstrain is UnboundConstrain ? ((UnboundConstrain)EntityConstrain).Subject : EntityAccessor.About);
                    subject.Arguments.Add(FallbackConstrain is UnboundConstrain ? ((UnboundConstrain)FallbackConstrain).Subject : EntityAccessor.UnboundGraphName);
                    _expressions.Add(new Alias(subject, new Identifier(EntityAccessor.About.Name + "S")));

                    Call predicate = new Call(MethodNames.If);
                    predicate.Arguments.Add(bound);
                    predicate.Arguments.Add(EntityConstrain.Predicate);
                    predicate.Arguments.Add(FallbackConstrain.Predicate);
                    _expressions.Add(new Alias(predicate, new Identifier(EntityAccessor.About.Name + "P")));

                    Call value = new Call(MethodNames.If);
                    value.Arguments.Add(bound);
                    value.Arguments.Add(EntityConstrain.Value);
                    value.Arguments.Add(FallbackConstrain.Value);
                    _expressions.Add(new Alias(value, new Identifier(EntityAccessor.About.Name + "O")));
                }

                return _expressions;
            }
        }

        /// <summary>Gets the target entity accessor.</summary>
        public StrongEntityAccessor EntityAccessor { get; private set; }

        /// <summary>Gets the entity constrain.</summary>
        public EntityConstrain EntityConstrain { get; set; }

        /// <summary>Gets the fallback entity constrain.</summary>
        public EntityConstrain FallbackConstrain { get; set; }
    }
}