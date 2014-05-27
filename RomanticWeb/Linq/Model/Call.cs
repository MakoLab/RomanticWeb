using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using NullGuard;
using RomanticWeb.Linq.Model.Navigators;

namespace RomanticWeb.Linq.Model
{
    #region Enums
    /// <summary>Enumerator of all possible query functions.</summary>
    public enum MethodNames:uint
    {
        #region Math operators
        /// <summary>Add (+)</summary>
        Add=0x80010000,

        /// <summary>Substraction (-)</summary>
        Substract=0x80020000,

        /// <summary>Multiplication (*)</summary>
        Multiply=0x80030000,

        /// <summary>Division (/)</summary>
        Divide=0x80040000,

        /// <summary>Add and assign (+=)</summary>
        AddAndAssign=0xC0010000,

        /// <summary>Substract and assign (-=)</summary>
        SubstractAndAssign=0xC0020000,

        /// <summary>Multiply and assign (*=)</summary>
        MultiplyAndAssign=0xC0030000,

        /// <summary>Divide and assign (/=)</summary>
        DivideAndAssign=0xC0040000,

        /// <summary>Modulo (%)</summary>
        Modulo=0x80050000,

        /// <summary>Modulo and assign (%=)</summary>
        ModuloAndAssign=0xC0050000,

        /// <summary>Bitiwse shift left (&lt;&lt;)</summary>
        BitwiseShiftLeft=0x80060000,

        /// <summary>Bitiwse shift left and assign (&lt;&lt;=)</summary>
        BitwiseShiftLeftAndAssign=0xC0006000,

        /// <summary>Bitiwse shift right (&gt;&gt;)</summary>
        BitwiseShiftRight=0x80070000,

        /// <summary>Bitiwse shift right and assign (&gt;&gt;=)</summary>
        BitwiseShiftRightAndAssign=0xC0070000,

        /// <summary>Bitiwse AND (&amp;)</summary>
        BitwiseAnd=0x81000000,

        /// <summary>Bitiwse AND and assign (&amp;=)</summary>
        BitwiseAndAndAssign=0xC1000000,

        /// <summary>Bitiwse OR (|)</summary>
        BitwiseOr=0x82000000,

        /// <summary>Bitiwse OR and assign (|=)</summary>
        BitwiseOrAndAssign=0xC2000000,

        /// <summary>Bitiwse NOT (~)</summary>
        BitwiseNot=0x83000000,

        /// <summary>Bitiwse NOT and assign (~=)</summary>
        BitwiseNotAndAssign=0xC3000000,

        /// <summary>Bitiwse exclusive OR (^)</summary>
        BitwiseXor=0x84000000,

        /// <summary>Bitiwse exclusive OR and assign (^=)</summary>
        BitwiseXorAndAssign=0xC4000000,
        #endregion
        #region Logical operators
        /// <summary>Logical AND (&amp;&amp;)</summary>
        And=0xA1000000,

        /// <summary>Logical OR (||)</summary>
        Or=0xA2000000,

        /// <summary>Logical NOT (!)</summary>
        Not=0xA3000000,

        /// <summary>Logical exclusive OR (^^)</summary>
        Xor=0xA4000000,
        #endregion
        #region Comparison operators
        /// <summary>Equals (==)</summary>
        Equal=0x95000000,

        /// <summary>Not equals (!=)</summary>
        NotEqual=0x96000000,

        /// <summary>Greater than (&gt;)</summary>
        GreaterThan=0x97000000,

        /// <summary>Greater than or equal (&gt;=)</summary>
        GreaterThanOrEqual=0x98000000,

        /// <summary>Less than (&lt;)</summary>
        LessThan=0x99000000,

        /// <summary>Less than or equal (&lt;=)</summary>
        LessThanOrEqual=0x9A000000,
        #endregion
        #region Functions
        /// <summary>Conversion.</summary>
        Convert=0x00000001,

        /// <summary>Absolute value.</summary>
        Abs=0x00000002,

        /// <summary>Average value.</summary>
        Avg=0x00000003,

        /// <summary>Sum of values.</summary>
        Sum=0x00000004,

        /// <summary>Count values.</summary>
        Count=0x00000005,

        /// <summary>In set operator.</summary>
        In=0x00000006,

        /// <summary>Exists operator.</summary>
        Any=0x00000007,

        /// <summary>Distinct.</summary>
        Distinct=0x00000008,

        /// <summary>Ceiling.</summary>
        Ceiling=0x00000009,

        /// <summary>Ceiling.</summary>
        Floor=0x0000000A,

        /// <summary>Round.</summary>
        Round=0x0000000B,

        /// <summary>Random integer number.</summary>
        RandomInt=0x0000000C,

        /// <summary>Random floating point number.</summary>
        RandomFloat=0x0000000D,

        /// <summary>Binds a constant to a variable.</summary>
        Bind=0x00000010,

        /// <summary>Checks if the given variable is bound.</summary>
        Bound=0x00000011,
        #endregion
        #region Result modifiers
        /// <summary>Offset operator.</summary>
        Offset=0x0000000E,

        /// <summary>Limit operator.</summary>
        Limit=0x0000000F,
        #endregion
        #region String functions
        /// <summary>String starts with.</summary>
        StartsWith=0x00000100,

        /// <summary>String ends with.</summary>
        EndsWith=0x00000200,

        /// <summary>String contains.</summary>
        Contains=0x00000300,

        /// <summary>String mathes regular expression.</summary>
        Regex=0x00000400,

        /// <summary>Replace string with string.</summary>
        Replace=0x00000500,

        /// <summary>Substring.</summary>
        Substring=0x00000600,

        /// <summary>Convertion to upper case.</summary>
        ToUpper=0x00000700,

        /// <summary>Convertion to lower case.</summary>
        ToLower=0x00000800,

        /// <summary>String length.</summary>
        Length=0x00000900,
        #endregion
        #region Date-time functions
        /// <summary>Gets current date and time.</summary>
        Now = 0x00001000,

        /// <summary>Gets current year.</summary>
        Year = 0x00002000,

        /// <summary>Gets current month.</summary>
        Month = 0x00003000,

        /// <summary>Gets current day of month.</summary>
        Day = 0x00004000,

        /// <summary>Gets current hour.</summary>
        Hour = 0x00005000,

        /// <summary>Gets current minute.</summary>
        Minute = 0x00006000,

        /// <summary>Gets current second.</summary>
        Second = 0x00007000,

        /// <summary>Gets current milisecond.</summary>
        Milisecond=0x00008000
        #endregion
    }
    #endregion

    /// <summary>Represents a function call in query.</summary>
    [QueryComponentNavigator(typeof(CallNavigator))]
    public class Call:QueryComponent,IExpression,ISelectableQueryComponent
    {
        #region Fields
        /// <summary>Helper for determining if the given function is an operator.</summary>
        public const MethodNames Operator=(MethodNames)0x80000000;

        /// <summary>Helper for determining if the given function is an operator with additional assignment.</summary>
        public const MethodNames AndAssign=(MethodNames)0x40000000;

        /// <summary>Helper for determining if the given function is logical.</summary>
        /// <remarks>This helper value is usually used in conjuction with <see cref="RomanticWeb.Linq.Model.MethodNames" /></remarks>
        public const MethodNames Logical=(MethodNames)0x20000000;

        /// <summary>Helper for determining if the given function is comparison.</summary>
        /// <remarks>This helper value is usually used in conjuction with <see cref="RomanticWeb.Linq.Model.MethodNames" /></remarks>
        public const MethodNames Comparison=(MethodNames)0x10000000;

        private IList<IExpression> _arguments;
        private MethodNames _member;
        #endregion

        #region Constructors
        /// <summary>Default constructor with method name passed.</summary>
        /// <param name="methodName">Method name.</param>
        public Call(MethodNames methodName):base()
        {
            ObservableCollection<IExpression> arguments=new ObservableCollection<IExpression>();
            arguments.CollectionChanged+=OnCollectionChanged;
            _arguments=arguments;
            _member=methodName;
        }
        #endregion

        #region Properties
        /// <summary>Gets a list of arguments.</summary>
        public IList<IExpression> Arguments { get { return _arguments; } }

        /// <summary>Gets a called member.</summary>
        public MethodNames Member { get { return _member; } }

        /// <summary>Gets an enumeration of selectable expressions.</summary>
        IEnumerable<IExpression> ISelectableQueryComponent.Expressions { get { return new IExpression[] { this }; } }
        #endregion

        #region Public methods
        /// <summary>Creates a string representation of this call.</summary>
        /// <returns>String representation of this call.</returns>
        public override string ToString()
        {
            string functionName=_member.ToString().ToUpper();
            string openingBracket="(";
            string closingBracket=")";
            string targetAccessor=System.String.Empty;
            string separator=System.String.Empty;
            IEnumerable<IExpression> arguments=_arguments;
            IExpression target=null;
            switch (_member)
            {
                case MethodNames.Any:
                    functionName="EXISTS";
                    openingBracket=closingBracket=" ";
                    break;
                case MethodNames.In:
                    target=(_arguments.Count>0?_arguments.First():null);
                    separator=targetAccessor=" ";
                    arguments=(_arguments.Count>1?_arguments.Skip(1):new IExpression[0]);
                    break;
            }

            return System.String.Format("{0}{1}{2}{3}{4}{5}{6}",target,targetAccessor,functionName,separator,openingBracket,System.String.Join(",",arguments),closingBracket);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals([AllowNull] object operand)
        {
            return (!Object.Equals(operand,null))&&(operand.GetType()==typeof(Call))&&(_member==(((Call)operand)._member))&&
                (_arguments.Count==((Call)operand)._arguments.Count)&&
                (_arguments.SequenceEqual((((Call)operand)._arguments)));
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int result=typeof(Call).FullName.GetHashCode()^_member.GetHashCode();
            foreach (IExpression expression in _arguments)
            {
                result^=expression.GetHashCode();
            }

            return result;
        }
        #endregion

        #region Non-public methods
        /// <summary>Rised when arguments collection has changed.</summary>
        /// <param name="sender">Sender of this event.</param>
        /// <param name="e">Eventarguments.</param>
        protected virtual void OnCollectionChanged(object sender,NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (QueryComponent queryComponent in e.NewItems)
                        {
                            if (queryComponent!=null)
                            {
                                queryComponent.OwnerQuery=OwnerQuery;
                            }
                        }

                        break;
                    }
            }
        }
        #endregion
    }
}