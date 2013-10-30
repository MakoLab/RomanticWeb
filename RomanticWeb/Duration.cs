//------------------------------------------------------------------------------ 
// <copyright company="Microsoft" file="XsdDuration.cs">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner primary="true" current="true"></owner> 
//-----------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace System
{
    /// <summary>This structure holds components of an Xsd Duration. It is used internally to support Xsd durations without loss of fidelity. Duration structures are immutable once they've been created.</summary> 
    public struct Duration
    {
        #region Fields
        private const uint NegativeBit=0x80000000;
        private int years;
        private int months;
        private int days;
        private int hours;
        private int minutes;
        private int seconds;
        private uint nanoseconds;
        #endregion

        #region Constructors
        /// <summary>Construct an Duration from time parts.</summary> 
        public Duration(int hours,int minutes,int seconds,int nanoseconds)
        {
            if (minutes<0)
            {
                throw new ArgumentOutOfRangeException("minutes");
            }

            if (seconds<0)
            {
                throw new ArgumentOutOfRangeException("seconds");
            }

            if ((nanoseconds<0)||(nanoseconds>999999999))
            {
                throw new ArgumentOutOfRangeException("nanoseconds");
            }

            this.years=0;
            this.months=0;
            this.days=0;
            this.hours=Math.Abs(hours);
            this.minutes=minutes;
            this.seconds=seconds;
            this.nanoseconds=(uint)nanoseconds;
            if (hours<0)
            {
                this.nanoseconds|=NegativeBit;
            }
        }

        /// <summary>Construct an Duration from time parts.</summary> 
        public Duration(bool isNegative,int hours,int minutes,int seconds,int nanoseconds)
        {
            if (hours<0)
            {
                throw new ArgumentOutOfRangeException("hours");
            }

            if (minutes<0)
            {
                throw new ArgumentOutOfRangeException("minutes");
            }

            if (seconds<0)
            {
                throw new ArgumentOutOfRangeException("seconds");
            }

            if ((nanoseconds<0)||(nanoseconds>999999999))
            {
                throw new ArgumentOutOfRangeException("nanoseconds");
            }

            this.years=0;
            this.months=0;
            this.days=0;
            this.hours=hours;
            this.minutes=minutes;
            this.seconds=seconds;
            this.nanoseconds=(uint)nanoseconds;
            if (isNegative)
            {
                this.nanoseconds|=NegativeBit;
            }
        }

        /// <summary>Construct an Duration from date parts.</summary> 
        public Duration(int years,int months,int days)
        {
            if (months<0)
            {
                throw new ArgumentOutOfRangeException("months");
            }

            if (days<0)
            {
                throw new ArgumentOutOfRangeException("days");
            }

            this.years=Math.Abs(years);
            this.months=months;
            this.days=days;
            this.hours=0;
            this.minutes=0;
            this.seconds=0;
            this.nanoseconds=(uint)0;
            if (years<0)
            {
                this.nanoseconds|=NegativeBit;
            }
        }

        /// <summary>Construct an Duration from date parts.</summary> 
        public Duration(bool isNegative,int years,int months,int days)
        {
            if (years<0)
            {
                throw new ArgumentOutOfRangeException("years");
            }

            if (months<0)
            {
                throw new ArgumentOutOfRangeException("months");
            }

            if (days<0)
            {
                throw new ArgumentOutOfRangeException("days");
            }

            this.years=years;
            this.months=months;
            this.days=days;
            this.hours=0;
            this.minutes=0;
            this.seconds=0;
            this.nanoseconds=(uint)0;
            if (isNegative)
            {
                this.nanoseconds|=NegativeBit;
            }
        }

        /// <summary>Construct an Duration from component parts.</summary> 
        public Duration(bool isNegative,int years,int months,int days,int hours,int minutes,int seconds,int nanoseconds)
        {
            if (years<0)
            {
                throw new ArgumentOutOfRangeException("years");
            }

            if (months<0)
            {
                throw new ArgumentOutOfRangeException("months");
            }

            if (days<0)
            {
                throw new ArgumentOutOfRangeException("days");
            }

            if (hours<0)
            {
                throw new ArgumentOutOfRangeException("hours");
            }

            if (minutes<0)
            {
                throw new ArgumentOutOfRangeException("minutes");
            }

            if (seconds<0)
            {
                throw new ArgumentOutOfRangeException("seconds");
            }

            if ((nanoseconds<0)||(nanoseconds>999999999))
            {
                throw new ArgumentOutOfRangeException("nanoseconds");
            }

            this.years=years;
            this.months=months;
            this.days=days;
            this.hours=hours;
            this.minutes=minutes;
            this.seconds=seconds;
            this.nanoseconds=(uint)nanoseconds;
            if (isNegative)
            {
                this.nanoseconds|=NegativeBit;
            }
        }

        /// <summary>Construct an Duration from a TimeSpan value.</summary> 
        public Duration(TimeSpan timeSpan):this(timeSpan,DurationType.Duration)
        {
        }

        /// <summary>Constructs an Duration from a string in the xsd:duration format. Components are stored with loss of fidelity (except in the case of overflow).</summary> 
        public Duration(string duration):this(duration,DurationType.Duration)
        {
        }

        /// <summary>Constructs an Duration from a string in the xsd:duration format. Components are stored without loss of fidelity (except in the case of overflow).</summary>
        public Duration(string duration,DurationType durationType)
        {
            Duration result;
            Exception exception=TryParse(duration,durationType,out result);
            if (exception!=null)
            {
                throw exception;
            }

            this.years=result.Years;
            this.months=result.Months;
            this.days=result.Days;
            this.hours=result.Hours;
            this.minutes=result.Minutes;
            this.seconds=result.Seconds;
            this.nanoseconds=(uint)result.Nanoseconds;
            if (result.IsNegative)
            {
                this.nanoseconds|=NegativeBit;
            }

            return;
        }

        /// <summary>Construct an Duration from a TimeSpan value that represents an xsd:duration, an xdt:dayTimeDuration, or an xdt:yearMonthDuration.</summary> 
        internal Duration(TimeSpan timeSpan,DurationType durationType)
        {
            long ticks=timeSpan.Ticks;
            ulong ticksPos;
            bool isNegative;

            if (ticks<0)
            {
                isNegative=true;
                ticksPos=(ulong)-ticks;
            }
            else
            {
                isNegative=false;
                ticksPos=(ulong)ticks;
            }

            if (durationType==DurationType.YearMonthDuration)
            {
                int years=(int)(ticksPos/((ulong)TimeSpan.TicksPerDay*365));
                int months=(int)((ticksPos%((ulong)TimeSpan.TicksPerDay*365))/((ulong)TimeSpan.TicksPerDay*30));
                if (months==12)
                {
                    years++;
                    months=0;
                }

                this=new Duration(isNegative,years,months,0,0,0,0,0);
            }
            else
            {
                this.nanoseconds=(uint)(ticksPos%10000000)*100;
                if (isNegative)
                {
                    this.nanoseconds|=NegativeBit;
                }

                this.years=0;
                this.months=0;
                this.days=(int)(ticksPos/(ulong)TimeSpan.TicksPerDay);
                this.hours=(int)((ticksPos/(ulong)TimeSpan.TicksPerHour)%24);
                this.minutes=(int)((ticksPos/(ulong)TimeSpan.TicksPerMinute)%60);
                this.seconds=(int)((ticksPos/(ulong)TimeSpan.TicksPerSecond)%60);
            }
        }
        #endregion

        #region Enums
        /// <summary>Determines the type of the duration.</summary>
        public enum DurationType
        {
            /// <summary>States that given duration has both date and time parts.</summary>
            Duration,

            /// <summary>States that given duration has date part only.</summary>
            YearMonthDuration,

            /// <summary>States that given duration has time part only.</summary>
            DayTimeDuration,
        }

        private enum Parts
        {
            HasNone=0,
            HasYears=1,
            HasMonths=2,
            HasDays=4,
            HasHours=8,
            HasMinutes=16,
            HasSeconds=32,
        }
        #endregion

        #region Properties
        /// <summary>Return true if this duration is negative.</summary> 
        public bool IsNegative { get { return (this.nanoseconds&NegativeBit)!=0; } }

        /// <summary>Return number of years in this duration (stored in 31 bits).</summary> 
        public int Years { get { return this.years; } }

        /// <summary>Return number of months in this duration (stored in 31 bits).</summary>
        public int Months { get { return this.months; } }

        /// <summary>Return number of days in this duration (stored in 31 bits). </summary>
        public int Days { get { return this.days; } }

        /// <summary>Return number of hours in this duration (stored in 31 bits).</summary>
        public int Hours { get { return this.hours; } }

        /// <summary>Return number of minutes in this duration (stored in 31 bits).</summary>
        public int Minutes { get { return this.minutes; } }

        /// <summary>Return number of seconds in this duration (stored in 31 bits).</summary> 
        public int Seconds { get { return this.seconds; } }

        /// <summary>Return number of nanoseconds in this duration.</summary> 
        public int Nanoseconds { get { return (int)(this.nanoseconds&~NegativeBit); } }

        /// <summary>Return number of microseconds in this duration.</summary>
        public int Microseconds { get { return Nanoseconds/1000; } }

        /// <summary>Return number of milliseconds in this duration.</summary> 
        public int Milliseconds { get { return Nanoseconds/1000000; } }
        #endregion

        #region Public methods
        /// <summary>Compares two durations for equality.</summary>
        /// <param name="operandA">Left operand.</param>
        /// <param name="operandB">Right operand.</param>
        /// <returns><b>true</b> if all duration components are equal, otherwise <b>false</b>.</returns>
        public static bool operator==(Duration operandA,Duration operandB)
        {
            return (operandA.years==operandB.years)&&
                (operandA.months==operandB.months)&&
                (operandA.days==operandB.days)&&
                (operandA.hours==operandB.hours)&&
                (operandA.minutes==operandB.minutes)&&
                (operandA.seconds==operandB.seconds)&&
                (operandA.nanoseconds==operandB.nanoseconds);
        }

        /// <summary>Compares two durations for inequality.</summary>
        /// <param name="operandA">Left operand.</param>
        /// <param name="operandB">Right operand.</param>
        /// <returns><b>true</b> if any of the duration components is different, otherwise <b>false</b>.</returns>
        public static bool operator!=(Duration operandA,Duration operandB)
        {
            return !(operandA==operandB);
        }

        /// <summary>Parses given string into a duration.</summary>
        /// <param name="duration">String with duration to be parsed.</param>
        /// <returns>Parsed duration.</returns>
        public static Duration Parse(string duration)
        {
            Duration result;
            Exception exception=TryParse(duration,DurationType.Duration,out result);
            if (exception!=null)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>Tries to parse a given duration string into a duration.</summary>
        /// <param name="duration">String with duration to be parsed.</param>
        /// <param name="result">Output value.</param>
        /// <returns><b>true</b> if the parsing was completed successfuly, otherwise <b>false</b>.</returns>
        public static bool TryParse(string duration,out Duration result)
        {
            Exception exception=TryParse(duration,DurationType.Duration,out result);
            return (exception==null);
        }

        /// <summary>Normalize year-month part and day-time part so that month &lt; 12, hour &lt; 24, minute &lt; 60, and second &lt; 60.</summary> 
        public Duration Normalize()
        {
            int years=Years;
            int months=Months;
            int days=Days;
            int hours=Hours;
            int minutes=Minutes;
            int seconds=Seconds;
            try
            {
                checked
                {
                    if (months>=12)
                    {
                        years+=months/12;
                        months%=12;
                    }

                    if (seconds>=60)
                    {
                        minutes+=seconds/60;
                        seconds%=60;
                    }

                    if (minutes>=60)
                    {
                        hours+=minutes/60;
                        minutes%=60;
                    }

                    if (hours>=24)
                    {
                        days+=hours/24;
                        hours%=24;
                    }
                }
            }
            catch (OverflowException)
            {
                throw new OverflowException();
            }

            return new Duration(IsNegative,years,months,days,hours,minutes,seconds,Nanoseconds);
        }

        /// <summary>Return the string representation of this duration.</summary> 
        public override string ToString()
        {
            return ToString(DurationType.Duration);
        }

        /// <summary>Internal helper method that converts an Xsd duration to a TimeSpan value. This code uses the estimate that there are 365 days in the year and 30 days in a month.</summary> 
        public TimeSpan ToTimeSpan()
        {
            return ToTimeSpan(DurationType.Duration);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return years.GetHashCode()^months.GetHashCode()^days.GetHashCode()^hours.GetHashCode()^minutes.GetHashCode()^seconds.GetHashCode()^nanoseconds.GetHashCode();
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals(object operand)
        {
            return (!Object.Equals(operand,null))&&(operand is Duration)&&(this==(Duration)operand);
        }
        #endregion

        #region Non-public methods
        internal static Exception TryParse(string s,DurationType durationType,out Duration result)
        {
            string errorCode;
            int length;
            int value,pos,numDigits;
            Parts parts=Parts.HasNone;
            result=new Duration();
            s=s.Trim();
            length=s.Length;
            pos=0;
            numDigits=0;
            if (pos>=length)
            {
                goto InvalidFormat;
            }

            if (s[pos]=='-')
            {
                pos++;
                result.nanoseconds=NegativeBit;
            }
            else
            {
                result.nanoseconds=0;
            }

            if (pos>=length)
            {
                goto InvalidFormat;
            }

            if (s[pos++]!='P')
            {
                goto InvalidFormat;
            }

            errorCode=TryParseDigits(s,ref pos,false,out value,out numDigits);
            if (errorCode!=null)
            {
                goto Error;
            }

            if (pos>=length)
            {
                goto InvalidFormat;
            }

            if (s[pos]=='Y')
            {
                if (numDigits==0)
                {
                    goto InvalidFormat;
                }

                parts|=Parts.HasYears;
                result.years=value;
                if (++pos==length)
                {
                    goto Done;
                }

                errorCode=TryParseDigits(s,ref pos,false,out value,out numDigits);
                if (errorCode!=null)
                {
                    goto Error;
                }

                if (pos>=length)
                {
                    goto InvalidFormat;
                }
            }

            if (s[pos]=='M')
            {
                if (numDigits==0)
                {
                    goto InvalidFormat;
                }

                parts|=Parts.HasMonths;
                result.months=value;
                if (++pos==length)
                {
                    goto Done;
                }

                errorCode=TryParseDigits(s,ref pos,false,out value,out numDigits);
                if (errorCode!=null)
                {
                    goto Error;
                }

                if (pos>=length)
                {
                    goto InvalidFormat;
                }
            }

            if (s[pos]=='D')
            {
                if (numDigits==0)
                {
                    goto InvalidFormat;
                }

                parts|=Parts.HasDays;
                result.days=value;
                if (++pos==length)
                {
                    goto Done;
                }

                errorCode=TryParseDigits(s,ref pos,false,out value,out numDigits);
                if (errorCode!=null)
                {
                    goto Error;
                }

                if (pos>=length)
                {
                    goto InvalidFormat;
                }
            }

            if (s[pos]=='T')
            {
                if (numDigits!=0)
                {
                    goto InvalidFormat;
                }

                pos++;
                errorCode=TryParseDigits(s,ref pos,false,out value,out numDigits);
                if (errorCode!=null)
                {
                    goto Error;
                }

                if (pos>=length)
                {
                    goto InvalidFormat;
                }

                if (s[pos]=='H')
                {
                    if (numDigits==0)
                    {
                        goto InvalidFormat;
                    }

                    parts|=Parts.HasHours;
                    result.hours=value;
                    if (++pos==length)
                    {
                        goto Done;
                    }

                    errorCode=TryParseDigits(s,ref pos,false,out value,out numDigits);
                    if (errorCode!=null)
                    {
                        goto Error;
                    }

                    if (pos>=length)
                    {
                        goto InvalidFormat;
                    }
                }

                if (s[pos]=='M')
                {
                    if (numDigits==0)
                    {
                        goto InvalidFormat;
                    }

                    parts|=Parts.HasMinutes;
                    result.minutes=value;
                    if (++pos==length)
                    {
                        goto Done;
                    }

                    errorCode=TryParseDigits(s,ref pos,false,out value,out numDigits);
                    if (errorCode!=null)
                    {
                        goto Error;
                    }

                    if (pos>=length)
                    {
                        goto InvalidFormat;
                    }
                }

                if (s[pos]=='.')
                {
                    pos++;
                    parts|=Parts.HasSeconds;
                    result.seconds=value;
                    errorCode=TryParseDigits(s,ref pos,true,out value,out numDigits);
                    if (errorCode!=null)
                    {
                        goto Error;
                    }

                    if (numDigits==0)
                    {
                        value=0;
                    }

                    for (; numDigits>9; numDigits--)
                    {
                        value/=10;
                    }

                    for (; numDigits<9; numDigits++)
                    {
                        value*=10;
                    }

                    result.nanoseconds|=(uint)value;
                    if (pos>=length)
                    {
                        goto InvalidFormat;
                    }

                    if (s[pos]!='S')
                    {
                        goto InvalidFormat;
                    }

                    if (++pos==length)
                    {
                        goto Done;
                    }
                }
                else if (s[pos]=='S')
                {
                    if (numDigits==0)
                    {
                        goto InvalidFormat;
                    }

                    parts|=Parts.HasSeconds;
                    result.seconds=value;
                    if (++pos==length)
                    {
                        goto Done;
                    }
                }
            }

            if (numDigits!=0)
            {
                goto InvalidFormat;
            }

            if (pos!=length)
            {
                goto InvalidFormat;
            }

        Done:
            if (parts==Parts.HasNone)
            {
                goto InvalidFormat;
            }

            if (durationType==DurationType.DayTimeDuration)
            {
                if ((parts&(Parts.HasYears|Parts.HasMonths))!=0)
                {
                    goto InvalidFormat;
                }
            }
            else if (durationType==DurationType.YearMonthDuration)
            {
                if ((parts&~(Duration.Parts.HasYears|Duration.Parts.HasMonths))!=0)
                {
                    goto InvalidFormat;
                }
            }

            return null;
        InvalidFormat:
            return new FormatException();
        Error:
            return new OverflowException();
        }

        /// <summary>Internal helper method that converts an Xsd duration to a TimeSpan value. This code uses the estimate that there are 365 days in the year and 30 days in a month.</summary>
        internal TimeSpan ToTimeSpan(DurationType durationType)
        {
            TimeSpan result;
            Exception exception=TryToTimeSpan(durationType,out result);
            if (exception!=null)
            {
                throw exception;
            }

            return result;
        }

        internal Exception TryToTimeSpan(out TimeSpan result)
        {
            return TryToTimeSpan(DurationType.Duration,out result);
        }

        internal Exception TryToTimeSpan(DurationType durationType,out TimeSpan result)
        {
            Exception exception=null;
            ulong ticks=0;
            try
            {
                checked
                {
                    if (durationType!=DurationType.DayTimeDuration)
                    {
                        ticks+=((ulong)this.years+((ulong)this.months/12))*365;
                        ticks+=((ulong)this.months%12)*30;
                    }

                    if (durationType!=DurationType.YearMonthDuration)
                    {
                        ticks+=(ulong)this.days;
                        ticks*=24;
                        ticks+=(ulong)this.hours;
                        ticks*=60;
                        ticks+=(ulong)this.minutes;
                        ticks*=60;
                        ticks+=(ulong)this.seconds;
                        ticks*=(ulong)TimeSpan.TicksPerSecond;
                        ticks+=(ulong)Nanoseconds/100;
                    }
                    else
                    {
                        ticks*=(ulong)TimeSpan.TicksPerDay;
                    }

                    if (IsNegative)
                    {
                        if (ticks==(ulong)Int64.MaxValue+1)
                        {
                            result=new TimeSpan(Int64.MinValue);
                        }
                        else
                        {
                            result=new TimeSpan(-((long)ticks));
                        }
                    }
                    else
                    {
                        result=new TimeSpan((long)ticks);
                    }

                    return null;
                }
            }
            catch (OverflowException)
            {
                result=TimeSpan.MinValue;
                exception=new OverflowException();
            }

            return exception;
        }

        /// <summary>Return the string representation according to xsd:duration rules, xdt:dayTimeDuration rules, or xdt:yearMonthDuration rules.</summary>
        internal string ToString(DurationType durationType)
        {
            StringBuilder sb=new StringBuilder(20);
            int nanoseconds,digit,zeroIdx,len;
            if (IsNegative)
            {
                sb.Append('-');
            }

            sb.Append('P');
            if (durationType!=DurationType.DayTimeDuration)
            {
                if (this.years!=0)
                {
                    sb.Append(XmlConvert.ToString(this.years));
                    sb.Append('Y');
                }

                if (this.months!=0)
                {
                    sb.Append(XmlConvert.ToString(this.months));
                    sb.Append('M');
                }
            }

            if (durationType!=DurationType.YearMonthDuration)
            {
                if (this.days!=0)
                {
                    sb.Append(XmlConvert.ToString(this.days));
                    sb.Append('D');
                }

                if ((this.hours!=0)||(this.minutes!=0)||(this.seconds!=0)||(Nanoseconds!=0))
                {
                    sb.Append('T');
                    if (this.hours!=0)
                    {
                        sb.Append(XmlConvert.ToString(this.hours));
                        sb.Append('H');
                    }

                    if (this.minutes!=0)
                    {
                        sb.Append(XmlConvert.ToString(this.minutes));
                        sb.Append('M');
                    }

                    nanoseconds=Nanoseconds;
                    if ((this.seconds!=0)||(nanoseconds!=0))
                    {
                        sb.Append(XmlConvert.ToString(this.seconds));
                        if (nanoseconds!=0)
                        {
                            sb.Append('.');
                            len=sb.Length;
                            sb.Length+=9;
                            zeroIdx=sb.Length-1;

                            for (int idx=zeroIdx; idx>=len; idx--)
                            {
                                digit=nanoseconds%10;
                                sb[idx]=(char)(digit+'0');
                                if (zeroIdx==idx&&digit==0)
                                {
                                    zeroIdx--;
                                }

                                nanoseconds/=10;
                            }

                            sb.Length=zeroIdx+1;
                        }

                        sb.Append('S');
                    }
                }

                if (sb[sb.Length-1]=='P')
                {
                    sb.Append("T0S");
                }
            }
            else
            {
                if (sb[sb.Length-1]=='P')
                {
                    sb.Append("0M");
                }
            }

            return sb.ToString();
        }

        private static string TryParseDigits(string s,ref int offset,bool eatDigits,out int result,out int numDigits)
        {
            int offsetStart=offset;
            int offsetEnd=s.Length;
            int digit;
            result=0;
            numDigits=0;
            while (offset<offsetEnd&&s[offset]>='0'&&s[offset]<='9')
            {
                digit=s[offset]-'0';
                if (result>(Int32.MaxValue-digit)/10)
                {
                    if (!eatDigits)
                    {
                        return "XmlConvert_Overflow";
                    }

                    numDigits=offset-offsetStart;
                    while (offset<offsetEnd&&s[offset]>='0'&&s[offset]<='9')
                    {
                        offset++;
                    }

                    return null;
                }

                result=(result*10)+digit;
                offset++;
            }

            numDigits=offset-offsetStart;
            return null;
        }
        #endregion
    }
}