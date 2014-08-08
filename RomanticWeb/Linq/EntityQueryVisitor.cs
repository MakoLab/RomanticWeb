using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Parsing;
using RomanticWeb.Entities;
using RomanticWeb.Linq.Expressions;
using RomanticWeb.Linq.Model;
using RomanticWeb.Linq.Model.Navigators;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Linq
{
    /// <summary>Visits query expressions.</summary>
    public class EntityQueryVisitor : ThrowingExpressionTreeVisitor, IQueryVisitor
    {
        #region Fields
        private readonly IEntityContext _entityContext;
        private Query _query;
        private Stack<IQueryComponentNavigator> _currentComponent;
        private QueryComponent _lastComponent;
        private string _itemNameOverride;
        #endregion

        #region Constructors
        /// <summary>Creates an instance of the query visitor.</summary>
        /// <param name="query">Query to be filled.</param>
        /// <param name="entityContext">Entity context.</param>
        internal EntityQueryVisitor(Query query, IEntityContext entityContext)
            : base()
        {
            _entityContext = entityContext;
            _currentComponent = new Stack<IQueryComponentNavigator>();
            _query = query;
            _itemNameOverride = null;
        }
        #endregion

        #region Properties
        /// <summary>Gets an associated query.</summary>
        Query IQueryVisitor.Query { get { return _query; } }

        /// <summary>Gets the mappings repository.</summary>
        IMappingsRepository IQueryVisitor.MappingsRepository { get { return _entityContext.Mappings; } }

        /// <summary>Gets the base Uri selection policy.</summary>
        IBaseUriSelectionPolicy IQueryVisitor.BaseUriSelector { get { return _entityContext.BaseUriSelector; } }

        /// <summary>Gets an associated query.</summary>
        internal Query Query { get { return _query; } }

        /// <summary>Gets or sets an item name to be used when creating entity accessors.</summary>
        internal string ItemNameOverride { get { return _itemNameOverride; } set { _itemNameOverride = value; } }

        /// <summary>Gets the mappings repository.</summary>
        internal IMappingsRepository MappingsRepository { get { return _entityContext.Mappings; } }

        /// <summary>Gets or sets an auxiliar constant from clause.</summary>
        internal IExpression ConstantFromClause { get; set; }
        #endregion

        #region Public methods
        /// <summary>Retrevies last visited and transformed query and prepares for next inspection.</summary>
        /// <returns>Query component visited or query itself.</returns>
        public QueryComponent RetrieveComponent()
        {
            CleanupComponent(_lastComponent);
            QueryComponent result = _lastComponent;
            if ((_currentComponent.Count > 0) && (_currentComponent.Peek().NavigatedComponent is BinaryOperator))
            {
                BinaryOperator binaryOperator = (BinaryOperator)_currentComponent.Peek().NavigatedComponent;
                if ((binaryOperator.LeftOperand != null) && (binaryOperator.RightOperand != null))
                {
                    _lastComponent = (QueryComponent)_currentComponent.Pop().NavigatedComponent;
                }
                else
                {
                    _lastComponent = (QueryComponent)_currentComponent.Peek().NavigatedComponent;
                }
            }
            else
            {
                _lastComponent = (_currentComponent.Count > 0 ? (QueryComponent)_currentComponent.Pop().NavigatedComponent : _query);
            }

            return result;
        }

        internal Remotion.Linq.Clauses.FromClauseBase GetMemberTarget(System.Linq.Expressions.MemberExpression expression)
        {
            Remotion.Linq.Clauses.FromClauseBase result = null;
            System.Linq.Expressions.MemberExpression memberExpression = expression.Expression as System.Linq.Expressions.MemberExpression;
            if ((memberExpression == null) && ((memberExpression = ((IQueryVisitor)this).TransformUnaryExpression(expression.Expression) as System.Linq.Expressions.MemberExpression) != null))
            {
                SafeVisitExpression(memberExpression);
            }

            if (memberExpression is System.Linq.Expressions.MemberExpression)
            {
                if (memberExpression.Member is PropertyInfo)
                {
                    PropertyInfo propertyInfo = (PropertyInfo)memberExpression.Member;
                    string itemName = propertyInfo.Name.CamelCase();
                    StrongEntityAccessor entityAccessor = _query.FindAllComponents<StrongEntityAccessor>().Where(item => item.SourceExpression.FromExpression == memberExpression).FirstOrDefault();
                    if (entityAccessor != null)
                    {
                        itemName = _query.RetrieveIdentifier(entityAccessor.About.Name);
                    }
                    else
                    {
                        SafeVisitExpression(memberExpression);
                    }

                    result = new FromPropertyClause(itemName, propertyInfo.PropertyType.FindItemType(), memberExpression);
                }
                else
                {
                    throw CreateUnhandledItemException<System.Linq.Expressions.MemberExpression>(expression, "GetMemberTarget");
                }
            }
            else
            {
                result = GetSourceExpression(expression);
            }

            return result;
        }
        #endregion

        #region Protected methods
        /// <summary>Visits a query source expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited.</returns>
        protected override System.Linq.Expressions.Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            Remotion.Linq.Clauses.FromClauseBase sourceExpression = GetSourceExpression(expression);
            StrongEntityAccessor entityAccessor = this.GetEntityAccessor(sourceExpression);
            if (entityAccessor != null)
            {
                _query.AddEntityAccessor(entityAccessor);
                _lastComponent = _query;
            }
            else
            {
                _lastComponent = (from entityConstrain in _query.FindAllComponents<EntityConstrain>()
                                  where entityConstrain.GetType() == typeof(EntityConstrain)
                                  let identifier = entityConstrain.Value as Identifier
                                  let targetExpression = ((IQueryVisitor)this).TransformFromExpression(sourceExpression).FromExpression
                                  where (identifier != null) && (targetExpression != null) && (entityConstrain.TargetExpression.EqualsTo(targetExpression))
                                  select identifier).FirstOrDefault();
                HandleComponent(_lastComponent);
            }

            return expression;
        }

        /// <summary>Visits a binary expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override System.Linq.Expressions.Expression VisitBinaryExpression(System.Linq.Expressions.BinaryExpression expression)
        {
            MethodNames operatorName;
            System.Linq.Expressions.ExpressionType expressionType = expression.NodeType;
            switch (expression.NodeType)
            {
                case System.Linq.Expressions.ExpressionType.OrElse:
                    expressionType = System.Linq.Expressions.ExpressionType.Or;
                    goto default;
                case System.Linq.Expressions.ExpressionType.AndAlso:
                    expressionType = System.Linq.Expressions.ExpressionType.And;
                    goto default;
                default:
                    string methodName = Enum.GetNames(typeof(MethodNames)).Where(item => item == expressionType.ToString()).FirstOrDefault();
                    if (!System.String.IsNullOrEmpty(methodName))
                    {
                        operatorName = (MethodNames)Enum.Parse(typeof(MethodNames), methodName);
                    }
                    else
                    {
                        return base.VisitBinaryExpression(expression);
                    }

                    break;
            }

            BinaryOperator binaryOperator = new BinaryOperator(operatorName);
            HandleComponent(binaryOperator);

            VisitExpression(expression.Left);
            CleanupComponent(_lastComponent);

            VisitExpression(expression.Right);
            CleanupComponent(_lastComponent);

            _lastComponent = binaryOperator;
            return expression;
        }

        /// <summary>Visits an unary expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override System.Linq.Expressions.Expression VisitUnaryExpression(System.Linq.Expressions.UnaryExpression expression)
        {
            MethodNames operatorName;
            System.Linq.Expressions.ExpressionType expressionType = expression.NodeType;
            switch (expression.NodeType)
            {
                case System.Linq.Expressions.ExpressionType.TypeAs:
                    return VisitExpression(expression.Operand);
                default:
                    string methodName = Enum.GetNames(typeof(MethodNames)).Where(item => item == expressionType.ToString()).FirstOrDefault();
                    if (!System.String.IsNullOrEmpty(methodName))
                    {
                        operatorName = (MethodNames)Enum.Parse(typeof(MethodNames), methodName);
                    }
                    else
                    {
                        return base.VisitUnaryExpression(expression);
                    }

                    break;
            }

            UnaryOperator unaryOperator = new UnaryOperator(operatorName);
            HandleComponent(unaryOperator);

            VisitExpression(expression.Operand);
            CleanupComponent(_lastComponent);

            _lastComponent = unaryOperator;
            return expression;
        }

        /// <summary>Visits a method call expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override System.Linq.Expressions.Expression VisitMethodCallExpression(System.Linq.Expressions.MethodCallExpression expression)
        {
            Call call = null;
            bool isEntityExtensionMethod = ((expression.Method.DeclaringType == typeof(EntityExtensions)) && (expression.Arguments.Count == 2) &&
                (expression.Arguments.Count == expression.Method.GetParameters().Length) && (expression.Arguments[1] is System.Linq.Expressions.ConstantExpression));
            switch (expression.Method.Name)
            {
                case "Next":
                    if (expression.Method.DeclaringType == typeof(Random)) { call = new Call(MethodNames.RandomInt); }
                    goto default;
                case "NextDouble":
                    if (expression.Method.DeclaringType == typeof(Random)) { call = new Call(MethodNames.RandomFloat); }
                    goto default;
                case "Abs":
                case "Round":
                case "Ceiling":
                case "Floor":
                    if (expression.Method.DeclaringType == typeof(Math)) { call = new Call((MethodNames)Enum.Parse(typeof(MethodNames), expression.Method.Name)); }
                    goto default;
                case "StartsWith":
                case "EndsWith":
                case "Contains":
                case "Substring":
                case "ToLower":
                case "ToUpper":
                    if (expression.Method.DeclaringType == typeof(string)) { call = new Call((MethodNames)Enum.Parse(typeof(MethodNames), expression.Method.Name)); }
                    goto default;
                case "IsMatch":
                    if (expression.Method.DeclaringType == typeof(Regex)) { call = new Call(MethodNames.Regex); }
                    goto default;
                case "Is":
                    if (isEntityExtensionMethod) { return VisitIsMethodCall(expression); }
                    goto default;
                case "Predicate":
                    if (isEntityExtensionMethod) { return VisitPredicateMethodCall(expression); }
                    goto default;
                default:
                    if (call == null)
                    {
                        return base.VisitMethodCallExpression(expression);
                    }

                    break;
            }

            HandleComponent(call);
            if ((!expression.Method.IsStatic) && (expression.Object != null))
            {
                VisitExpression(expression.Object);
                CleanupComponent(_lastComponent);
            }

            foreach (System.Linq.Expressions.Expression argument in expression.Arguments)
            {
                VisitExpression(argument);
                CleanupComponent(_lastComponent);
            }

            _lastComponent = call;
            return expression;
        }

        /// <summary>Visits an <see cref="EntityExtensions.Is(RomanticWeb.Entities.IEntity,System.Uri)" /> method call.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Returns visited expression.</returns>
        protected virtual System.Linq.Expressions.Expression VisitIsMethodCall(System.Linq.Expressions.MethodCallExpression expression)
        {
            object objectValue = ((System.Linq.Expressions.ConstantExpression)expression.Arguments[1]).Value;
            if ((objectValue is IEnumerable) && (objectValue.GetType().FindItemType() == typeof(EntityId)))
            {
                var types = (IEnumerable<EntityId>)objectValue;
                int count = types.Count();
                if (count > 0)
                {
                    StrongEntityAccessor entityAccessor = this.GetEntityAccessor(this.GetSourceExpression(expression.Arguments[0]));
                    if (entityAccessor != null)
                    {
                        EntityTypeConstrain constrain = new EntityTypeConstrain(
                            types.Select(item => item.Uri).First(),
                            entityAccessor.SourceExpression.FromExpression,
                            (count > 1 ? types.Skip(1).Select(item => item.Uri).ToArray() : new Uri[0]));
                        entityAccessor.Elements.Add(constrain);
                        _lastComponent = constrain;
                    }
                }

                return expression;
            }
            else
            {
                return base.VisitMethodCallExpression(expression);
            }
        }

        /// <summary>Visits an <see cref="EntityExtensions.Predicate" /> method call.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Returns visited expression.</returns>
        protected virtual System.Linq.Expressions.Expression VisitPredicateMethodCall(System.Linq.Expressions.MethodCallExpression expression)
        {
            object objectValue = ((System.Linq.Expressions.ConstantExpression)expression.Arguments[1]).Value;
            if (objectValue is Uri)
            {
                Uri predicate = (Uri)objectValue;
                if (!predicate.IsAbsoluteUri)
                {
                    predicate = new Uri(_entityContext.BaseUriSelector.SelectBaseUri(new EntityId(predicate)), predicate.ToString());
                }

                VisitExpression(expression.Arguments[0]);
                StrongEntityAccessor entityAccessor = null;
                Remotion.Linq.Clauses.FromClauseBase sourceExpression = null;
                if (expression.Arguments[0] is System.Linq.Expressions.MemberExpression)
                {
                    System.Linq.Expressions.MemberExpression memberExpression = (System.Linq.Expressions.MemberExpression)expression.Arguments[0];
                    if (memberExpression.Member is PropertyInfo)
                    {
                        PropertyInfo propertyInfo = (PropertyInfo)memberExpression.Member;
                        sourceExpression = new FromPropertyClause(propertyInfo.Name, propertyInfo.PropertyType, memberExpression);
                        _query.AddEntityAccessor(entityAccessor = this.GetEntityAccessor(sourceExpression));
                    }
                    else
                    {
                        return base.VisitMethodCallExpression(expression);
                    }
                }
                else
                {
                    entityAccessor = this.GetEntityAccessor(GetSourceExpression(expression.Arguments[0]));
                }

                if (entityAccessor != null)
                {
                    Type type = null;
                    string name = null;
                    if (predicate == Rdf.subject)
                    {
                        name = "Id";
                        type = typeof(IEntity);
                    }
                    else
                    {
                        IPropertyMapping propertyMapping = _entityContext.Mappings.MappingFor(expression.Arguments[0].Type).Properties.FirstOrDefault(item => item.Uri.AbsoluteUri == predicate.AbsoluteUri);

                        if (propertyMapping == null)
                        {
                            ExceptionHelper.ThrowMappingException(predicate);
                        }

                        if (propertyMapping.DeclaringType != expression.Arguments[0].Type)
                        {
                            propertyMapping = _entityContext.Mappings.MappingFor(propertyMapping.DeclaringType).Properties.FirstOrDefault(item => item.Uri.AbsoluteUri == predicate.AbsoluteUri);

                            if (propertyMapping == null)
                            {
                                ExceptionHelper.ThrowMappingException(predicate);
                            }
                        }

                        type = propertyMapping.EntityMapping.EntityType;
                        name = propertyMapping.Name;
                    }

                    System.Linq.Expressions.MemberExpression propertyExpression = System.Linq.Expressions.Expression.Property(expression.Arguments[0], type, name);
                    return VisitMemberExpression(propertyExpression);
                }

                return expression;
            }
            else
            {
                return base.VisitMethodCallExpression(expression);
            }
        }

        /// <summary>Visits a member expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override System.Linq.Expressions.Expression VisitMemberExpression(System.Linq.Expressions.MemberExpression expression)
        {
            if ((expression.Member.Name == "Id") && (typeof(IEntity).IsAssignableFrom(expression.Member.DeclaringType)))
            {
                Remotion.Linq.Clauses.FromClauseBase target = GetMemberTarget(expression);
                if (target != null)
                {
                    return VisitEntityId(new EntityIdentifierExpression(expression, target));
                }
                else
                {
                    ExceptionHelper.ThrowInvalidCastException(typeof(IEntity), expression.Member.DeclaringType);
                }
            }
            else if ((typeof(IEntity).IsAssignableFrom(expression.Member.DeclaringType)) && (expression.Member is PropertyInfo))
            {
                IPropertyMapping propertyMapping = _entityContext.Mappings.FindPropertyMapping((PropertyInfo)expression.Member);
                Remotion.Linq.Clauses.FromClauseBase target = GetMemberTarget(expression);
                if ((propertyMapping != null) && (target != null))
                {
                    return VisitEntityProperty(new EntityPropertyExpression(expression, propertyMapping, target, (_itemNameOverride != null ? _itemNameOverride : expression.Member.Name)));
                }
                else
                {
                    ExceptionHelper.ThrowInvalidCastException(typeof(IEntity), expression.Member.DeclaringType);
                }
            }
            else if (expression.Member is PropertyInfo)
            {
                return VisitPropertyExpression(expression);
            }

            return base.VisitMemberExpression(expression);
        }

        /// <summary>Visits a property expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected virtual System.Linq.Expressions.Expression VisitPropertyExpression(System.Linq.Expressions.MemberExpression expression)
        {
            var propertyInfo = (PropertyInfo)expression.Member;
            Call call = null;
            bool isParameterles = true;
            switch (propertyInfo.Name)
            {
                case "Length":
                    if (propertyInfo.DeclaringType == typeof(string))
                    {
                        call = new Call((MethodNames)Enum.Parse(typeof(MethodNames), propertyInfo.Name));
                        isParameterles = false;
                    }

                    goto default;
                case "Year":
                case "Month":
                case "Day":
                case "Hour":
                case "Minute":
                case "Second":
                case "Millisecond":
                    if (propertyInfo.DeclaringType == typeof(DateTime))
                    {
                        call = new Call((MethodNames)Enum.Parse(typeof(MethodNames), propertyInfo.Name));
                        isParameterles = true;
                    }

                    goto default;
                case "Now":
                    if (propertyInfo.DeclaringType == typeof(DateTime)) { call = new Call((MethodNames)Enum.Parse(typeof(MethodNames), propertyInfo.Name)); }
                    goto default;
                case "Today":
                    if (propertyInfo.DeclaringType == typeof(DateTime)) { call = new Call(MethodNames.Now); }
                    goto default;
                case "Uri":
                    if (typeof(EntityId).IsAssignableFrom(propertyInfo.DeclaringType)) { return VisitEntityIdUri((System.Linq.Expressions.MemberExpression)expression.Expression); }
                    goto default;
                default:
                    if (call == null)
                    {
                        return base.VisitMemberExpression(expression);
                    }

                    break;
            }

            HandleComponent(call);
            if (!isParameterles)
            {
                VisitExpression(expression.Expression);
                CleanupComponent(_lastComponent);
            }

            _lastComponent = call;

            return expression;
        }

        /// <summary>Visits a constant expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override System.Linq.Expressions.Expression VisitConstantExpression(System.Linq.Expressions.ConstantExpression expression)
        {
            if ((expression.Value is IEnumerable) && (!(expression.Value is string)))
            {
                List list = new List();
                IEnumerable value = (IEnumerable)expression.Value;
                foreach (object item in value)
                {
                    if (item is IEntity)
                    {
                        list.Values.Add(new Literal(((IEntity)item).Id.Uri));
                    }
                    else if (item is EntityId)
                    {
                        list.Values.Add(new Literal(((EntityId)item).Uri));
                    }
                    else
                    {
                        list.Values.Add(new Literal(ResolveRelativeUriIfNecessery(item)));
                    }
                }

                _lastComponent = list;
            }
            else if (expression.Value is IEntity)
            {
                HandleComponent(_lastComponent = new Literal(((IEntity)expression.Value).Id.Uri));
            }
            else if (expression.Value != null)
            {
                HandleComponent(_lastComponent = new Literal(ResolveRelativeUriIfNecessery(expression.Value)));
            }
            else
            {
                Type valueType = FindLiteralType();
                HandleComponent(_lastComponent = new Literal(valueType));
            }

            return expression;
        }

        /// <summary>Visits a sub-query expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override System.Linq.Expressions.Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            Remotion.Linq.Clauses.FromClauseBase sourceExpression = (Remotion.Linq.Clauses.FromClauseBase)((QuerySourceReferenceExpression)expression.QueryModel.SelectClause.Selector).ReferencedQuerySource;
            StrongEntityAccessor entityAccessor = (sourceExpression.FromExpression is System.Linq.Expressions.ConstantExpression ? null : this.GetEntityAccessor(sourceExpression));
            if (entityAccessor != null)
            {
                Query query = _query.CreateSubQuery(entityAccessor.About);
                query.AddEntityAccessor(entityAccessor);
                EntityQueryModelVisitor queryModelVisitor = new EntityQueryModelVisitor(query, _entityContext);
                queryModelVisitor.VisitQueryModel(expression.QueryModel);
                _lastComponent = queryModelVisitor.Result;
                HandleComponent(_lastComponent);
            }
            else
            {
                NonEntityQueryModelVisitor queryModelVisitor = new NonEntityQueryModelVisitor(this, ConstantFromClause);
                Stack<IQueryComponentNavigator> currentComponent = _currentComponent;
                _currentComponent = new Stack<IQueryComponentNavigator>();
                queryModelVisitor.VisitQueryModel(expression.QueryModel);
                _currentComponent = currentComponent;
                _lastComponent = queryModelVisitor.Result;
                HandleComponent(_lastComponent);
            }

            return expression;
        }

        /// <summary>Visits an Uri property expression called on <see cref="EntityId" />.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected virtual System.Linq.Expressions.Expression VisitEntityIdUri(System.Linq.Expressions.MemberExpression expression)
        {
            bool isInSelectScenario = (_currentComponent.Count == 0);
            StrongEntityAccessor entityAccessor = (isInSelectScenario ? this.GetEntityAccessor(GetMemberTarget(expression)) : this.GetEntityAccessor(GetSourceExpression(expression)));
            _query.AddEntityAccessor(entityAccessor);

            if (isInSelectScenario)
            {
                _lastComponent = entityAccessor;
            }
            else
            {
                foreach (var entityConstrain in entityAccessor.Elements.OfType<EntityConstrain>())
                {
                    if (entityConstrain.Value is Identifier)
                    {
                        Identifier identifier = (Identifier)entityConstrain.Value;
                        string constrainIdentifier = _query.RetrieveIdentifier(identifier.Name);
                        string propertyIdentifier = null;
                        Type propertyType = null;
                        if (expression.Expression is System.Linq.Expressions.MemberExpression)
                        {
                            PropertyInfo propertyInfo = (PropertyInfo)((System.Linq.Expressions.MemberExpression)expression.Expression).Member;
                            propertyIdentifier = _query.CreateIdentifier(propertyInfo.Name);
                            propertyType = propertyInfo.PropertyType;
                        }
                        else if (expression.Expression is Remotion.Linq.Clauses.Expressions.QuerySourceReferenceExpression)
                        {
                            Remotion.Linq.Clauses.Expressions.QuerySourceReferenceExpression querySource = (Remotion.Linq.Clauses.Expressions.QuerySourceReferenceExpression)expression.Expression;
                            propertyIdentifier = querySource.ReferencedQuerySource.ItemName;
                            propertyType = querySource.ReferencedQuerySource.ItemType;
                        }

                        if ((constrainIdentifier == propertyIdentifier) && (identifier.NativeType == propertyType))
                        {
                            _lastComponent = identifier;
                            break;
                        }
                    }
                }

                if (_lastComponent == null)
                {
                    _lastComponent = entityAccessor.About;
                }

                HandleComponent(_lastComponent);
            }

            return expression;
        }

        /// <summary>Visits an entity identifier member.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected virtual System.Linq.Expressions.Expression VisitEntityId(EntityIdentifierExpression expression)
        {
            if ((_currentComponent.Count > 0) && (_currentComponent.Peek() is BinaryOperatorNavigator))
            {
                StrongEntityAccessor entityAccessor = this.GetEntityAccessor(expression.Target);
                HandleComponent(entityAccessor.About);
                BinaryOperator binaryOperator = ((BinaryOperatorNavigator)_currentComponent.Peek()).NavigatedComponent;
                Filter filter = new Filter(binaryOperator);
                _query.AddEntityAccessor(entityAccessor);
                entityAccessor.Elements.Add(filter);
                _currentComponent.Push(filter.GetQueryComponentNavigator());
                _lastComponent = filter;
            }
            else
            {
                _lastComponent = this.GetEntityAccessor(expression.Target).About;
            }

            return expression;
        }

        /// <summary>Visits an entity member.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected virtual System.Linq.Expressions.Expression VisitEntityProperty(EntityPropertyExpression expression)
        {
            StrongEntityAccessor entityAccessor = this.GetEntityAccessor(expression.Target);
            _query.AddEntityAccessor(entityAccessor);
            Identifier memberIdentifier = null;
            EntityConstrain constrain = _query.FindAllComponents<EntityConstrain>()
                .FirstOrDefault(item => (item.GetType() == typeof(EntityConstrain) && (item.TargetExpression.EqualsTo(expression.Expression))));
            if (constrain == null)
            {
                memberIdentifier = (_query.FindAllComponents<StrongEntityAccessor>()
                    .Where(item => item.SourceExpression.FromExpression.EqualsTo(expression.Expression))
                    .Select(item => item.About).FirstOrDefault()) ?? (new Identifier(_query.CreateVariableName(expression.Name), expression.EntityProperty.PropertyType));
                constrain = new EntityConstrain(new Literal(expression.PropertyMapping.Uri), memberIdentifier, expression.Expression);
                if (!entityAccessor.Elements.Contains(constrain))
                {
                    entityAccessor.Elements.Add(constrain);
                }
            }
            else
            {
                if (constrain.ShouldBeOptional(_currentComponent))
                {
                    entityAccessor.Elements.Remove(constrain);
                    OptionalPattern optional = new OptionalPattern();
                    optional.Patterns.Add(constrain);
                    entityAccessor.Elements.Add(optional);
                }

                memberIdentifier = (Identifier)constrain.Value;
            }

            HandleComponent(memberIdentifier);
            _lastComponent = memberIdentifier;
            return expression;
        }

        /// <summary>Visits a type binary expression.</summary>
        /// <param name="expression">Expression to be visited.</param>
        /// <returns>Expression visited</returns>
        protected override System.Linq.Expressions.Expression VisitTypeBinaryExpression(System.Linq.Expressions.TypeBinaryExpression expression)
        {
            var classMappings = MappingsRepository.FindMappedClasses(expression.TypeOperand);
            if (classMappings.Any())
            {
                Remotion.Linq.Clauses.FromClauseBase sourceExpression = GetSourceExpression(expression.Expression);
                StrongEntityAccessor entityAccessor = _query.FindAllComponents<StrongEntityAccessor>().FirstOrDefault(item => item.SourceExpression == sourceExpression);
                if (entityAccessor == null)
                {
                    entityAccessor = this.GetEntityAccessor(sourceExpression);
                    _query.Elements.Add(entityAccessor);
                }

                EntityTypeConstrain typeConstrain = this.CreateTypeConstrain(expression.TypeOperand, entityAccessor.SourceExpression.FromExpression);
                _lastComponent = typeConstrain;
                if ((_currentComponent.Count > 0) && (_currentComponent.Peek() is BinaryOperatorNavigator))
                {
                    HandleComponent(typeConstrain);
                }
                else if (!entityAccessor.Elements.Contains(typeConstrain))
                {
                    entityAccessor.Elements.Add(typeConstrain);
                }
            }
            else
            {
                return base.VisitTypeBinaryExpression(expression);
            }

            return expression;
        }

        /// <summary>Visits a unhandled expression.</summary>
        /// <param name="unhandledItem">Expression beeing unhandled.</param>
        /// <param name="visitMethod">Visitor method.</param>
        /// <returns>Expression visited</returns>
        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            System.Linq.Expressions.Expression expression = unhandledItem as System.Linq.Expressions.Expression;
            return new NotSupportedException(expression != null ? FormattingExpressionTreeVisitor.Format(expression) : unhandledItem.ToString());
        }
        #endregion

        #region Private methods
        private void HandleComponent(QueryComponent component)
        {
            if (_currentComponent.Count > 0)
            {
                if (_currentComponent.Peek().CanAddComponent(component))
                {
                    if (!_currentComponent.Peek().ContainsComponent(component))
                    {
                        _currentComponent.Peek().AddComponent(component);
                    }
                }
                else
                {
                    _currentComponent.Pop();
                }
            }

            IQueryComponentNavigator queryComponentNavigator = component.GetQueryComponentNavigator();
            if (queryComponentNavigator != null)
            {
                _currentComponent.Push(queryComponentNavigator);
            }
        }

        private void CleanupComponent(QueryComponent component)
        {
            IQueryComponentNavigator queryComponentNavigator = _lastComponent.GetQueryComponentNavigator();
            if (queryComponentNavigator != null)
            {
                while ((_currentComponent.Count > 0) && (!_currentComponent.Peek().Equals(queryComponentNavigator)))
                {
                    _currentComponent.Pop();
                }

                if (_currentComponent.Count > 0)
                {
                    _currentComponent.Pop();
                }
            }
        }

        private Remotion.Linq.Clauses.FromClauseBase GetSourceExpression(System.Linq.Expressions.Expression expression)
        {
            Remotion.Linq.Clauses.FromClauseBase result = null;
            System.Linq.Expressions.Expression currentExpression = expression;
            bool isChange = false;
            while (currentExpression != null)
            {
                if (currentExpression is System.Linq.Expressions.MemberExpression)
                {
                    System.Linq.Expressions.MemberExpression memberExpression = (System.Linq.Expressions.MemberExpression)currentExpression;
                    if (!(memberExpression.Member is PropertyInfo))
                    {
                        throw CreateUnhandledItemException<System.Linq.Expressions.MemberExpression>(memberExpression, "GetSourceExpression");
                    }

                    currentExpression = memberExpression.Expression;
                    isChange = true;
                }
                else if (currentExpression is Remotion.Linq.Clauses.Expressions.QuerySourceReferenceExpression)
                {
                    result = (Remotion.Linq.Clauses.FromClauseBase)((Remotion.Linq.Clauses.Expressions.QuerySourceReferenceExpression)currentExpression).ReferencedQuerySource;
                    if (!typeof(IQueryable).IsAssignableFrom(result.FromExpression.Type))
                    {
                        Stack<IQueryComponentNavigator> currentComponent = _currentComponent;
                        _currentComponent = new Stack<IQueryComponentNavigator>();
                        VisitExpression(result.FromExpression);
                        _currentComponent = currentComponent;
                        isChange = true;
                    }

                    break;
                }

                if (isChange)
                {
                    isChange = false;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        private object ResolveRelativeUriIfNecessery(object value)
        {
            if ((value != null) && (_entityContext.BaseUriSelector != null))
            {
                if (value is Uri)
                {
                    Uri uri = (Uri)value;
                    if (!uri.IsAbsoluteUri)
                    {
                        value = new Uri(_entityContext.BaseUriSelector.SelectBaseUri(new EntityId(uri)), uri);
                    }
                }
                else if (value is EntityId)
                {
                    EntityId entityId = (EntityId)value;
                    if (!entityId.Uri.IsAbsoluteUri)
                    {
                        value = entityId.MakeAbsolute(_entityContext.BaseUriSelector.SelectBaseUri(entityId));
                    }
                }
            }

            return value;
        }

        private Type FindLiteralType()
        {
            Type result = null;
            foreach (IQueryComponentNavigator componentNavigator in _currentComponent)
            {
                result = FindLiteralType(componentNavigator.NavigatedComponent);
                if (result != null)
                {
                    break;
                }
            }

            return result;
        }

        private Type FindLiteralType(IQueryComponent component)
        {
            Type result = null;
            if (component != null)
            {
                if (component is BinaryOperator)
                {
                    BinaryOperator binaryOperator = (BinaryOperator)component;
                    result = FindLiteralType(binaryOperator.LeftOperand);
                    if (result == null)
                    {
                        result = FindLiteralType(binaryOperator.RightOperand);
                    }
                }
                else if (component is Identifier)
                {
                    result = ((Identifier)component).NativeType;
                }
                else if (component is Alias)
                {
                    result = FindLiteralType(((Alias)component).Component);
                }
            }

            return result;
        }

        private void SafeVisitExpression(System.Linq.Expressions.Expression expression)
        {
            Stack<IQueryComponentNavigator> currentStack = _currentComponent;
            QueryComponent lastComponent = _lastComponent;
            _currentComponent = new Stack<IQueryComponentNavigator>();
            VisitExpression(expression);
            _currentComponent = currentStack;
            _lastComponent = lastComponent;
        }
        #endregion
    }
}