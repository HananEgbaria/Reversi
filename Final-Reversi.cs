// Dafny program Final-Reversi.dfy compiled into C#
// To recompile, use 'csc' with: /r:System.Numerics.dll
// and choosing /target:exe or /target:library
// You might also want to include compiler switches like:
//     /debug /nowarn:0164 /nowarn:0219

using System; // for Func
using System.Numerics;

namespace Dafny
{
  using System.Collections.Generic;
// set this option if you want to use System.Collections.Immutable and if you know what you're doing.
#if DAFNY_USE_SYSTEM_COLLECTIONS_IMMUTABLE
  using System.Collections.Immutable;
  using System.Linq;

  public class Set<T>
  {
    readonly ImmutableHashSet<T> setImpl;
    Set(ImmutableHashSet<T> d) {
      this.setImpl = d;
    }
    public static readonly Set<T> Empty = new Set<T>(ImmutableHashSet<T>.Empty);
    public static Set<T> FromElements(params T[] values) {
      return FromElements((IEnumerable<T>)values);
    }
    public static Set<T> FromElements(IEnumerable<T> values) {
      var d = ImmutableHashSet<T>.Empty.ToBuilder();
      foreach (T t in values)
        d.Add(t);
      return new Set<T>(d.ToImmutable());
    }
    public static Set<T> FromCollection(ICollection<T> values) {
      var d = ImmutableHashSet<T>.Empty.ToBuilder();
      foreach (T t in values)
        d.Add(t);
      return new Set<T>(d.ToImmutable());
    }
    public int Length {
      get { return this.setImpl.Count; }
    }
    public long LongLength {
      get { return this.setImpl.Count; }
    }
    public IEnumerable<T> Elements {
      get {
        return this.setImpl;
      }
    }
    /// <summary>
    /// This is an inefficient iterator for producing all subsets of "this".  Each set returned is the same
    /// Set<T> object (but this Set<T> object is fresh; in particular, it is not "this").
    /// </summary>
    public IEnumerable<Set<T>> AllSubsets {
      get {
        // Start by putting all set elements into a list
        var elmts = new List<T>();
        elmts.AddRange(this.setImpl);
        var n = elmts.Count;
        var which = new bool[n];
        var s = ImmutableHashSet<T>.Empty.ToBuilder();
        while (true) {
          yield return new Set<T>(s.ToImmutable());
          // "add 1" to "which", as if doing a carry chain.  For every digit changed, change the membership of the corresponding element in "s".
          int i = 0;
          for (; i < n && which[i]; i++) {
            which[i] = false;
            s.Remove(elmts[i]);
          }
          if (i == n) {
            // we have cycled through all the subsets
            break;
          }
          which[i] = true;
          s.Add(elmts[i]);
        }
      }
    }
    public bool Equals(Set<T> other) {
        return this.setImpl.SetEquals(other.setImpl);
    }
    public override bool Equals(object other) {
        var otherSet = other as Set<T>;
        return otherSet != null && this.Equals(otherSet);
    }
    public override int GetHashCode() {
      var hashCode = 1;
      foreach (var t in this.setImpl) {
        hashCode = hashCode * (t.GetHashCode()+3);
      }
      return hashCode;
    }
    public override string ToString() {
      var s = "{";
      var sep = "";
      foreach (var t in this.setImpl) {
        s += sep + t.ToString();
        sep = ", ";
      }
      return s + "}";
    }
    public bool IsProperSubsetOf(Set<T> other) {
        return IsProperSubsetOf(other);
    }
    public bool IsSubsetOf(Set<T> other) {
        return IsSubsetOf(other);
    }
    public bool IsSupersetOf(Set<T> other) {
      return other.IsSupersetOf(this);
    }
    public bool IsProperSupersetOf(Set<T> other) {
      return other.IsProperSupersetOf(this);
    }
    public bool IsDisjointFrom(Set<T> other) {
      ImmutableHashSet<T> a, b;
      if (this.setImpl.Count < other.setImpl.Count) {
        a = this.setImpl; b = other.setImpl;
      } else {
        a = other.setImpl; b = this.setImpl;
      }
      foreach (T t in a) {
        if (b.Contains(t))
          return false;
      }
      return true;
    }
    public bool Contains(T t) {
      return this.setImpl.Contains(t);
    }
    public Set<T> Union(Set<T> other) {
        return new Set<T>(this.setImpl.Union(other.setImpl));
    }
    public Set<T> Intersect(Set<T> other) {
      return new Set<T>(this.setImpl.Intersect(other.setImpl));
    }
    public Set<T> Difference(Set<T> other) {
        return new Set<T>(this.setImpl.Except(other.setImpl));
    }
  }
  public partial class MultiSet<T>
  {

    readonly ImmutableDictionary<T, int> dict;
    MultiSet(ImmutableDictionary<T, int> d) {
      dict = d;
    }
    public static readonly MultiSet<T> Empty = new MultiSet<T>(ImmutableDictionary<T, int>.Empty);
    public static MultiSet<T> FromElements(params T[] values) {
      var d = ImmutableDictionary<T, int>.Empty.ToBuilder();
      foreach (T t in values) {
        var i = 0;
        if (!d.TryGetValue(t, out i)) {
          i = 0;
        }
        d[t] = i + 1;
      }
      return new MultiSet<T>(d.ToImmutable());
    }
    public static MultiSet<T> FromCollection(ICollection<T> values) {
      var d = ImmutableDictionary<T, int>.Empty.ToBuilder();
      foreach (T t in values) {
        var i = 0;
        if (!d.TryGetValue(t, out i)) {
          i = 0;
        }
        d[t] = i + 1;
      }
      return new MultiSet<T>(d.ToImmutable());
    }
    public static MultiSet<T> FromSeq(Sequence<T> values) {
      var d = ImmutableDictionary<T, int>.Empty.ToBuilder();
      foreach (T t in values.Elements) {
        var i = 0;
        if (!d.TryGetValue(t, out i)) {
          i = 0;
        }
        d[t] = i + 1;
      }
      return new MultiSet<T>(d.ToImmutable());
    }
    public static MultiSet<T> FromSet(Set<T> values) {
      var d = ImmutableDictionary<T, int>.Empty.ToBuilder();
      foreach (T t in values.Elements) {
        d[t] = 1;
      }
      return new MultiSet<T>(d.ToImmutable());
    }

    public bool Equals(MultiSet<T> other) {
      return other.IsSubsetOf(this) && this.IsSubsetOf(other);
    }
    public override bool Equals(object other) {
      return other is MultiSet<T> && Equals((MultiSet<T>)other);
    }
    public override int GetHashCode() {
      var hashCode = 1;
      foreach (var kv in dict) {
        var key = kv.Key.GetHashCode();
        key = (key << 3) | (key >> 29) ^ kv.Value.GetHashCode();
        hashCode = hashCode * (key + 3);
      }
      return hashCode;
    }
    public override string ToString() {
      var s = "multiset{";
      var sep = "";
      foreach (var kv in dict) {
        var t = kv.Key.ToString();
        for (int i = 0; i < kv.Value; i++) {
          s += sep + t.ToString();
          sep = ", ";
        }
      }
      return s + "}";
    }
    public bool IsProperSubsetOf(MultiSet<T> other) {
      return !Equals(other) && IsSubsetOf(other);
    }
    public bool IsSubsetOf(MultiSet<T> other) {
      foreach (T t in dict.Keys) {
        if (!other.dict.ContainsKey(t) || other.dict[t] < dict[t])
          return false;
      }
      return true;
    }
    public bool IsSupersetOf(MultiSet<T> other) {
      return other.IsSubsetOf(this);
    }
    public bool IsProperSupersetOf(MultiSet<T> other) {
      return other.IsProperSubsetOf(this);
    }
    public bool IsDisjointFrom(MultiSet<T> other) {
      foreach (T t in dict.Keys) {
        if (other.dict.ContainsKey(t))
          return false;
      }
      foreach (T t in other.dict.Keys) {
        if (dict.ContainsKey(t))
          return false;
      }
      return true;
    }
    public bool Contains(T t) {
      return dict.ContainsKey(t);
    }
    public MultiSet<T> Union(MultiSet<T> other) {
      if (dict.Count == 0)
        return other;
      else if (other.dict.Count == 0)
        return this;
        var r = ImmutableDictionary<T, int>.Empty.ToBuilder();
      foreach (T t in dict.Keys) {
        var i = 0;
        if (!r.TryGetValue(t, out i)) {
          i = 0;
        }
        r[t] = i + dict[t];
      }
      foreach (T t in other.dict.Keys) {
        var i = 0;
        if (!r.TryGetValue(t, out i)) {
          i = 0;
        }
        r[t] = i + other.dict[t];
      }
      return new MultiSet<T>(r.ToImmutable());
    }
    public MultiSet<T> Intersect(MultiSet<T> other) {
      if (dict.Count == 0)
        return this;
      else if (other.dict.Count == 0)
        return other;
      var r = ImmutableDictionary<T, int>.Empty.ToBuilder();
      foreach (T t in dict.Keys) {
        if (other.dict.ContainsKey(t)) {
          r[t] = other.dict[t] < dict[t] ? other.dict[t] : dict[t];
        }
      }
      return new MultiSet<T>(r.ToImmutable());
    }
    public MultiSet<T> Difference(MultiSet<T> other) { // \result == this - other
      if (dict.Count == 0)
        return this;
      else if (other.dict.Count == 0)
        return this;
      var r = ImmutableDictionary<T, int>.Empty.ToBuilder();
      foreach (T t in dict.Keys) {
        if (!other.dict.ContainsKey(t)) {
          r[t] = dict[t];
        } else if (other.dict[t] < dict[t]) {
          r[t] = dict[t] - other.dict[t];
        }
      }
      return new MultiSet<T>(r.ToImmutable());
    }
    public IEnumerable<T> Elements {
      get {
        foreach (T t in dict.Keys) {
          int n;
          dict.TryGetValue(t, out n);
          for (int i = 0; i < n; i ++) {
            yield return t;
          }
        }
      }
    }
  }

  public partial class Map<U, V>
  {
    readonly ImmutableDictionary<U, V> dict;
    Map(ImmutableDictionary<U, V> d) {
      dict = d;
    }
    public static readonly Map<U, V> Empty = new Map<U, V>(ImmutableDictionary<U, V>.Empty);
    public static Map<U, V> FromElements(params Pair<U, V>[] values) {
      var d = ImmutableDictionary<U, V>.Empty.ToBuilder();
      foreach (Pair<U, V> p in values) {
        d[p.Car] = p.Cdr;
      }
      return new Map<U, V>(d.ToImmutable());
    }
    public static Map<U, V> FromCollection(List<Pair<U, V>> values) {
      var d = ImmutableDictionary<U, V>.Empty.ToBuilder();
      foreach (Pair<U, V> p in values) {
        d[p.Car] = p.Cdr;
      }
      return new Map<U, V>(d.ToImmutable());
    }
    public int Length {
      get { return dict.Count; }
    }
    public long LongLength {
      get { return dict.Count; }
    }
    public bool Equals(Map<U, V> other) {
      foreach (U u in dict.Keys) {
        V v1, v2;
        if (!dict.TryGetValue(u, out v1)) {
          return false; // this shouldn't happen
        }
        if (!other.dict.TryGetValue(u, out v2)) {
          return false; // other dictionary does not contain this element
        }
        if (!v1.Equals(v2)) {
          return false;
        }
      }
      foreach (U u in other.dict.Keys) {
        if (!dict.ContainsKey(u)) {
          return false; // this shouldn't happen
        }
      }
      return true;
    }
    public override bool Equals(object other) {
      return other is Map<U, V> && Equals((Map<U, V>)other);
    }
    public override int GetHashCode() {
      var hashCode = 1;
      foreach (var kv in dict) {
        var key = kv.Key.GetHashCode();
        key = (key << 3) | (key >> 29) ^ kv.Value.GetHashCode();
        hashCode = hashCode * (key + 3);
      }
      return hashCode;
    }
    public override string ToString() {
      var s = "map[";
      var sep = "";
      foreach (var kv in dict) {
        s += sep + kv.Key.ToString() + " := " + kv.Value.ToString();
        sep = ", ";
      }
      return s + "]";
    }
    public bool IsDisjointFrom(Map<U, V> other) {
      foreach (U u in dict.Keys) {
        if (other.dict.ContainsKey(u))
          return false;
      }
      foreach (U u in other.dict.Keys) {
        if (dict.ContainsKey(u))
          return false;
      }
      return true;
    }
    public bool Contains(U u) {
      return dict.ContainsKey(u);
    }
    public V Select(U index) {
      return dict[index];
    }
    public Map<U, V> Update(U index, V val) {
      return new Map<U, V>(dict.SetItem(index, val));
    }
    public IEnumerable<U> Domain {
      get {
        return dict.Keys;
      }
    }
  }
#else // !def DAFNY_USE_SYSTEM_COLLECTIONS_IMMUTABLE
  public class Set<T>
  {
    HashSet<T> set;
    Set(HashSet<T> s) {
      this.set = s;
    }
    public static Set<T> Empty {
      get {
        return new Set<T>(new HashSet<T>());
      }
    }
    public static Set<T> FromElements(params T[] values) {
      var s = new HashSet<T>();
      foreach (T t in values)
        s.Add(t);
      return new Set<T>(s);
    }
    public static Set<T> FromCollection(ICollection<T> values) {
      HashSet<T> s = new HashSet<T>();
      foreach (T t in values)
        s.Add(t);
      return new Set<T>(s);
    }
    public int Length {
      get { return this.set.Count; }
    }
    public long LongLength {
      get { return this.set.Count; }
    }
    public IEnumerable<T> Elements {
      get {
        return this.set;
      }
    }
    /// <summary>
    /// This is an inefficient iterator for producing all subsets of "this".  Each set returned is the same
    /// Set<T> object (but this Set<T> object is fresh; in particular, it is not "this").
    /// </summary>
    public IEnumerable<Set<T>> AllSubsets {
      get {
        // Start by putting all set elements into a list
        var elmts = new List<T>();
        elmts.AddRange(this.set);
        var n = elmts.Count;
        var which = new bool[n];
        var s = new Set<T>(new HashSet<T>());
        while (true) {
          yield return s;
          // "add 1" to "which", as if doing a carry chain.  For every digit changed, change the membership of the corresponding element in "s".
          int i = 0;
          for (; i < n && which[i]; i++) {
            which[i] = false;
            s.set.Remove(elmts[i]);
          }
          if (i == n) {
            // we have cycled through all the subsets
            break;
          }
          which[i] = true;
          s.set.Add(elmts[i]);
        }
      }
    }
    public bool Equals(Set<T> other) {
      return this.set.Count == other.set.Count && IsSubsetOf(other);
    }
    public override bool Equals(object other) {
      return other is Set<T> && Equals((Set<T>)other);
    }
    public override int GetHashCode() {
      var hashCode = 1;
      foreach (var t in this.set) {
        hashCode = hashCode * (t.GetHashCode()+3);
      }
      return hashCode;
    }
    public override string ToString() {
      var s = "{";
      var sep = "";
      foreach (var t in this.set) {
        s += sep + t.ToString();
        sep = ", ";
      }
      return s + "}";
    }
    public bool IsProperSubsetOf(Set<T> other) {
      return this.set.Count < other.set.Count && IsSubsetOf(other);
    }
    public bool IsSubsetOf(Set<T> other) {
      if (other.set.Count < this.set.Count)
        return false;
      foreach (T t in this.set) {
        if (!other.set.Contains(t))
          return false;
      }
      return true;
    }
    public bool IsSupersetOf(Set<T> other) {
      return other.IsSubsetOf(this);
    }
    public bool IsProperSupersetOf(Set<T> other) {
      return other.IsProperSubsetOf(this);
    }
    public bool IsDisjointFrom(Set<T> other) {
      HashSet<T> a, b;
      if (this.set.Count < other.set.Count) {
        a = this.set; b = other.set;
      } else {
        a = other.set; b = this.set;
      }
      foreach (T t in a) {
        if (b.Contains(t))
          return false;
      }
      return true;
    }
    public bool Contains(T t) {
      return this.set.Contains(t);
    }
    public Set<T> Union(Set<T> other) {
      if (this.set.Count == 0)
        return other;
      else if (other.set.Count == 0)
        return this;
      HashSet<T> a, b;
      if (this.set.Count < other.set.Count) {
        a = this.set; b = other.set;
      } else {
        a = other.set; b = this.set;
      }
      var r = new HashSet<T>();
      foreach (T t in b)
        r.Add(t);
      foreach (T t in a)
        r.Add(t);
      return new Set<T>(r);
    }
    public Set<T> Intersect(Set<T> other) {
      if (this.set.Count == 0)
        return this;
      else if (other.set.Count == 0)
        return other;
      HashSet<T> a, b;
      if (this.set.Count < other.set.Count) {
        a = this.set; b = other.set;
      } else {
        a = other.set; b = this.set;
      }
      var r = new HashSet<T>();
      foreach (T t in a) {
        if (b.Contains(t))
          r.Add(t);
      }
      return new Set<T>(r);
    }
    public Set<T> Difference(Set<T> other) {
      if (this.set.Count == 0)
        return this;
      else if (other.set.Count == 0)
        return this;
      var r = new HashSet<T>();
      foreach (T t in this.set) {
        if (!other.set.Contains(t))
          r.Add(t);
      }
      return new Set<T>(r);
    }
  }
  public class MultiSet<T>
  {
    Dictionary<T, int> dict;
    MultiSet(Dictionary<T, int> d) {
      dict = d;
    }
    public static MultiSet<T> Empty {
      get {
        return new MultiSet<T>(new Dictionary<T, int>(0));
      }
    }
    public static MultiSet<T> FromElements(params T[] values) {
      Dictionary<T, int> d = new Dictionary<T, int>(values.Length);
      foreach (T t in values) {
        var i = 0;
        if (!d.TryGetValue(t, out i)) {
          i = 0;
        }
        d[t] = i + 1;
      }
      return new MultiSet<T>(d);
    }
    public static MultiSet<T> FromCollection(ICollection<T> values) {
      Dictionary<T, int> d = new Dictionary<T, int>();
      foreach (T t in values) {
        var i = 0;
        if (!d.TryGetValue(t, out i)) {
          i = 0;
        }
        d[t] = i + 1;
      }
      return new MultiSet<T>(d);
    }
    public static MultiSet<T> FromSeq(Sequence<T> values) {
      Dictionary<T, int> d = new Dictionary<T, int>();
      foreach (T t in values.Elements) {
        var i = 0;
        if (!d.TryGetValue(t, out i)) {
          i = 0;
        }
        d[t] = i + 1;
      }
      return new MultiSet<T>(d);
    }
    public static MultiSet<T> FromSet(Set<T> values) {
      Dictionary<T, int> d = new Dictionary<T, int>();
      foreach (T t in values.Elements) {
        d[t] = 1;
      }
      return new MultiSet<T>(d);
    }

    public bool Equals(MultiSet<T> other) {
      return other.IsSubsetOf(this) && this.IsSubsetOf(other);
    }
    public override bool Equals(object other) {
      return other is MultiSet<T> && Equals((MultiSet<T>)other);
    }
    public override int GetHashCode() {
      var hashCode = 1;
      foreach (var kv in dict) {
        var key = kv.Key.GetHashCode();
        key = (key << 3) | (key >> 29) ^ kv.Value.GetHashCode();
        hashCode = hashCode * (key + 3);
      }
      return hashCode;
    }
    public override string ToString() {
      var s = "multiset{";
      var sep = "";
      foreach (var kv in dict) {
        var t = kv.Key.ToString();
        for (int i = 0; i < kv.Value; i++) {
          s += sep + t.ToString();
          sep = ", ";
        }
      }
      return s + "}";
    }
    public bool IsProperSubsetOf(MultiSet<T> other) {
      return !Equals(other) && IsSubsetOf(other);
    }
    public bool IsSubsetOf(MultiSet<T> other) {
      foreach (T t in dict.Keys) {
        if (!other.dict.ContainsKey(t) || other.dict[t] < dict[t])
          return false;
      }
      return true;
    }
    public bool IsSupersetOf(MultiSet<T> other) {
      return other.IsSubsetOf(this);
    }
    public bool IsProperSupersetOf(MultiSet<T> other) {
      return other.IsProperSubsetOf(this);
    }
    public bool IsDisjointFrom(MultiSet<T> other) {
      foreach (T t in dict.Keys) {
        if (other.dict.ContainsKey(t))
          return false;
      }
      foreach (T t in other.dict.Keys) {
        if (dict.ContainsKey(t))
          return false;
      }
      return true;
    }
    public bool Contains(T t) {
      return dict.ContainsKey(t);
    }
    public MultiSet<T> Union(MultiSet<T> other) {
      if (dict.Count == 0)
        return other;
      else if (other.dict.Count == 0)
        return this;
      var r = new Dictionary<T, int>();
      foreach (T t in dict.Keys) {
        var i = 0;
        if (!r.TryGetValue(t, out i)) {
          i = 0;
        }
        r[t] = i + dict[t];
      }
      foreach (T t in other.dict.Keys) {
        var i = 0;
        if (!r.TryGetValue(t, out i)) {
          i = 0;
        }
        r[t] = i + other.dict[t];
      }
      return new MultiSet<T>(r);
    }
    public MultiSet<T> Intersect(MultiSet<T> other) {
      if (dict.Count == 0)
        return this;
      else if (other.dict.Count == 0)
        return other;
      var r = new Dictionary<T, int>();
      foreach (T t in dict.Keys) {
        if (other.dict.ContainsKey(t)) {
          r.Add(t, other.dict[t] < dict[t] ? other.dict[t] : dict[t]);
        }
      }
      return new MultiSet<T>(r);
    }
    public MultiSet<T> Difference(MultiSet<T> other) { // \result == this - other
      if (dict.Count == 0)
        return this;
      else if (other.dict.Count == 0)
        return this;
      var r = new Dictionary<T, int>();
      foreach (T t in dict.Keys) {
        if (!other.dict.ContainsKey(t)) {
          r.Add(t, dict[t]);
        } else if (other.dict[t] < dict[t]) {
          r.Add(t, dict[t] - other.dict[t]);
        }
      }
      return new MultiSet<T>(r);
    }
    public IEnumerable<T> Elements {
      get {
        List<T> l = new List<T>();
        foreach (T t in dict.Keys) {
          int n;
          dict.TryGetValue(t, out n);
          for (int i = 0; i < n; i ++) {
            l.Add(t);
          }
        }
        return l;
      }
    }
  }

  public class Map<U, V>
  {
    Dictionary<U, V> dict;
    Map(Dictionary<U, V> d) {
      dict = d;
    }
    public static Map<U, V> Empty {
      get {
        return new Map<U, V>(new Dictionary<U,V>());
      }
    }
    public static Map<U, V> FromElements(params Pair<U, V>[] values) {
      Dictionary<U, V> d = new Dictionary<U, V>(values.Length);
      foreach (Pair<U, V> p in values) {
        d[p.Car] = p.Cdr;
      }
      return new Map<U, V>(d);
    }
    public static Map<U, V> FromCollection(List<Pair<U, V>> values) {
      Dictionary<U, V> d = new Dictionary<U, V>(values.Count);
      foreach (Pair<U, V> p in values) {
        d[p.Car] = p.Cdr;
      }
      return new Map<U, V>(d);
    }
    public int Length {
      get { return dict.Count; }
    }
    public long LongLength {
      get { return dict.Count; }
    }
    public bool Equals(Map<U, V> other) {
      foreach (U u in dict.Keys) {
        V v1, v2;
        if (!dict.TryGetValue(u, out v1)) {
          return false; // this shouldn't happen
        }
        if (!other.dict.TryGetValue(u, out v2)) {
          return false; // other dictionary does not contain this element
        }
        if (!v1.Equals(v2)) {
          return false;
        }
      }
      foreach (U u in other.dict.Keys) {
        if (!dict.ContainsKey(u)) {
          return false; // this shouldn't happen
        }
      }
      return true;
    }
    public override bool Equals(object other) {
      return other is Map<U, V> && Equals((Map<U, V>)other);
    }
    public override int GetHashCode() {
      var hashCode = 1;
      foreach (var kv in dict) {
        var key = kv.Key.GetHashCode();
        key = (key << 3) | (key >> 29) ^ kv.Value.GetHashCode();
        hashCode = hashCode * (key + 3);
      }
      return hashCode;
    }
    public override string ToString() {
      var s = "map[";
      var sep = "";
      foreach (var kv in dict) {
        s += sep + kv.Key.ToString() + " := " + kv.Value.ToString();
        sep = ", ";
      }
      return s + "]";
    }
    public bool IsDisjointFrom(Map<U, V> other) {
      foreach (U u in dict.Keys) {
        if (other.dict.ContainsKey(u))
          return false;
      }
      foreach (U u in other.dict.Keys) {
        if (dict.ContainsKey(u))
          return false;
      }
      return true;
    }
    public bool Contains(U u) {
      return dict.ContainsKey(u);
    }
    public V Select(U index) {
      return dict[index];
    }
    public Map<U, V> Update(U index, V val) {
      Dictionary<U, V> d = new Dictionary<U, V>(dict);
      d[index] = val;
      return new Map<U, V>(d);
    }
    public IEnumerable<U> Domain {
      get {
        return dict.Keys;
      }
    }
  }
#endif
  public class Sequence<T>
  {
    T[] elmts;
    public Sequence(T[] ee) {
      elmts = ee;
    }
    public static Sequence<T> Empty {
      get {
        return new Sequence<T>(new T[0]);
      }
    }
    public static Sequence<T> FromElements(params T[] values) {
      return new Sequence<T>(values);
    }
    public static Sequence<char> FromString(string s) {
      return new Sequence<char>(s.ToCharArray());
    }
    public int Length {
      get { return elmts.Length; }
    }
    public long LongLength {
      get { return elmts.LongLength; }
    }
    public T[] Elements {
      get {
        return elmts;
      }
    }
    public IEnumerable<T> UniqueElements {
      get {
        var st = Set<T>.FromElements(elmts);
        return st.Elements;
      }
    }
    public T Select(ulong index) {
      return elmts[index];
    }
    public T Select(long index) {
      return elmts[index];
    }
    public T Select(uint index) {
      return elmts[index];
    }
    public T Select(int index) {
      return elmts[index];
    }
    public T Select(BigInteger index) {
      return elmts[(int)index];
    }
    public Sequence<T> Update(long index, T t) {
      T[] a = (T[])elmts.Clone();
      a[index] = t;
      return new Sequence<T>(a);
    }
    public Sequence<T> Update(ulong index, T t) {
      return Update((long)index, t);
    }
    public Sequence<T> Update(BigInteger index, T t) {
      return Update((long)index, t);
    }
    public bool Equals(Sequence<T> other) {
      int n = elmts.Length;
      return n == other.elmts.Length && EqualUntil(other, n);
    }
    public override bool Equals(object other) {
      return other is Sequence<T> && Equals((Sequence<T>)other);
    }
    public override int GetHashCode() {
      if (elmts == null || elmts.Length == 0)
        return 0;
      var hashCode = 0;
      for (var i = 0; i < elmts.Length; i++) {
        hashCode = (hashCode << 3) | (hashCode >> 29) ^ elmts[i].GetHashCode();
      }
      return hashCode;
    }
    public override string ToString() {
      if (elmts is char[]) {
        var s = "";
        foreach (var t in elmts) {
          s += t.ToString();
        }
        return s;
      } else {
        var s = "[";
        var sep = "";
        foreach (var t in elmts) {
          s += sep + t.ToString();
          sep = ", ";
        }
        return s + "]";
      }
    }
    bool EqualUntil(Sequence<T> other, int n) {
      for (int i = 0; i < n; i++) {
        if (!elmts[i].Equals(other.elmts[i]))
          return false;
      }
      return true;
    }
    public bool IsProperPrefixOf(Sequence<T> other) {
      int n = elmts.Length;
      return n < other.elmts.Length && EqualUntil(other, n);
    }
    public bool IsPrefixOf(Sequence<T> other) {
      int n = elmts.Length;
      return n <= other.elmts.Length && EqualUntil(other, n);
    }
    public Sequence<T> Concat(Sequence<T> other) {
      if (elmts.Length == 0)
        return other;
      else if (other.elmts.Length == 0)
        return this;
      T[] a = new T[elmts.Length + other.elmts.Length];
      System.Array.Copy(elmts, 0, a, 0, elmts.Length);
      System.Array.Copy(other.elmts, 0, a, elmts.Length, other.elmts.Length);
      return new Sequence<T>(a);
    }
    public bool Contains(T t) {
      int n = elmts.Length;
      for (int i = 0; i < n; i++) {
        if (t.Equals(elmts[i]))
          return true;
      }
      return false;
    }
    public Sequence<T> Take(long m) {
      if (elmts.LongLength == m)
        return this;
      T[] a = new T[m];
      System.Array.Copy(elmts, a, m);
      return new Sequence<T>(a);
    }
    public Sequence<T> Take(ulong n) {
      return Take((long)n);
    }
    public Sequence<T> Take(BigInteger n) {
      return Take((long)n);
    }
    public Sequence<T> Drop(long m) {
      if (m == 0)
        return this;
      T[] a = new T[elmts.Length - m];
      System.Array.Copy(elmts, m, a, 0, elmts.Length - m);
      return new Sequence<T>(a);
    }
    public Sequence<T> Drop(ulong n) {
      return Drop((long)n);
    }
    public Sequence<T> Drop(BigInteger n) {
      if (n.IsZero)
        return this;
      return Drop((long)n);
    }
  }
  public struct Pair<A, B>
  {
    public readonly A Car;
    public readonly B Cdr;
    public Pair(A a, B b) {
      this.Car = a;
      this.Cdr = b;
    }
  }
  public partial class Helpers {
    public static System.Predicate<BigInteger> PredicateConverter_byte(System.Predicate<byte> pred) {
      return x => pred((byte)x);
    }
    public static System.Predicate<BigInteger> PredicateConverter_sbyte(System.Predicate<sbyte> pred) {
      return x => pred((sbyte)x);
    }
    public static System.Predicate<BigInteger> PredicateConverter_ushort(System.Predicate<ushort> pred) {
      return x => pred((ushort)x);
    }
    public static System.Predicate<BigInteger> PredicateConverter_short(System.Predicate<short> pred) {
      return x => pred((short)x);
    }
    public static System.Predicate<BigInteger> PredicateConverter_uint(System.Predicate<uint> pred) {
      return x => pred((uint)x);
    }
    public static System.Predicate<BigInteger> PredicateConverter_int(System.Predicate<int> pred) {
      return x => pred((int)x);
    }
    public static System.Predicate<BigInteger> PredicateConverter_ulong(System.Predicate<ulong> pred) {
      return x => pred((ulong)x);
    }
    public static System.Predicate<BigInteger> PredicateConverter_long(System.Predicate<long> pred) {
      return x => pred((long)x);
    }
    // Computing forall/exists quantifiers
    public static bool QuantBool(bool frall, System.Predicate<bool> pred) {
      if (frall) {
        return pred(false) && pred(true);
      } else {
        return pred(false) || pred(true);
      }
    }
    public static bool QuantChar(bool frall, System.Predicate<char> pred) {
      for (int i = 0; i < 0x10000; i++) {
        if (pred((char)i) != frall) { return !frall; }
      }
      return frall;
    }
    public static bool QuantInt(BigInteger lo, BigInteger hi, bool frall, System.Predicate<BigInteger> pred) {
      for (BigInteger i = lo; i < hi; i++) {
        if (pred(i) != frall) { return !frall; }
      }
      return frall;
    }
    public static bool QuantSet<U>(Dafny.Set<U> set, bool frall, System.Predicate<U> pred) {
      foreach (var u in set.Elements) {
        if (pred(u) != frall) { return !frall; }
      }
      return frall;
    }
    public static bool QuantMap<U,V>(Dafny.Map<U,V> map, bool frall, System.Predicate<U> pred) {
      foreach (var u in map.Domain) {
        if (pred(u) != frall) { return !frall; }
      }
      return frall;
    }
    public static bool QuantSeq<U>(Dafny.Sequence<U> seq, bool frall, System.Predicate<U> pred) {
      foreach (var u in seq.Elements) {
        if (pred(u) != frall) { return !frall; }
      }
      return frall;
    }
    public static bool QuantDatatype<U>(IEnumerable<U> set, bool frall, System.Predicate<U> pred) {
      foreach (var u in set) {
        if (pred(u) != frall) { return !frall; }
      }
      return frall;
    }
    // Enumerating other collections
    public delegate Dafny.Set<T> ComprehensionDelegate<T>();
    public delegate Dafny.Map<U, V> MapComprehensionDelegate<U, V>();
    public static IEnumerable<bool> AllBooleans {
      get {
        yield return false;
        yield return true;
      }
    }
    public static IEnumerable<char> AllChars {
      get {
        for (int i = 0; i < 0x10000; i++) {
          yield return (char)i;
        }
      }
    }
    public static IEnumerable<BigInteger> AllIntegers {
      get {
        yield return new BigInteger(0);
        for (var j = new BigInteger(1);; j++) {
          yield return j;
          yield return -j;
        }
      }
    }
    public static IEnumerable<BigInteger> IntegerRange(Nullable<BigInteger> lo, Nullable<BigInteger> hi) {
      if (lo == null) {
        for (var j = (BigInteger)hi; true; ) {
          j--;
          yield return j;
        }
      } else if (hi == null) {
        for (var j = (BigInteger)lo; true; j++) {
          yield return j;
        }
      } else {
        for (var j = (BigInteger)lo; j < hi; j++) {
          yield return j;
        }
      }
    }
    // pre: b != 0
    // post: result == a/b, as defined by Euclidean Division (http://en.wikipedia.org/wiki/Modulo_operation)
    public static sbyte EuclideanDivision_sbyte(sbyte a, sbyte b) {
      return (sbyte)EuclideanDivision_int(a, b);
    }
    public static short EuclideanDivision_short(short a, short b) {
      return (short)EuclideanDivision_int(a, b);
    }
    public static int EuclideanDivision_int(int a, int b) {
      if (0 <= a) {
        if (0 <= b) {
          // +a +b: a/b
          return (int)(((uint)(a)) / ((uint)(b)));
        } else {
          // +a -b: -(a/(-b))
          return -((int)(((uint)(a)) / ((uint)(unchecked(-b)))));
        }
      } else {
        if (0 <= b) {
          // -a +b: -((-a-1)/b) - 1
          return -((int)(((uint)(-(a + 1))) / ((uint)(b)))) - 1;
        } else {
          // -a -b: ((-a-1)/(-b)) + 1
          return ((int)(((uint)(-(a + 1))) / ((uint)(unchecked(-b))))) + 1;
        }
      }
    }
    public static long EuclideanDivision_long(long a, long b) {
      if (0 <= a) {
        if (0 <= b) {
          // +a +b: a/b
          return (long)(((ulong)(a)) / ((ulong)(b)));
        } else {
          // +a -b: -(a/(-b))
          return -((long)(((ulong)(a)) / ((ulong)(unchecked(-b)))));
        }
      } else {
        if (0 <= b) {
          // -a +b: -((-a-1)/b) - 1
          return -((long)(((ulong)(-(a + 1))) / ((ulong)(b)))) - 1;
        } else {
          // -a -b: ((-a-1)/(-b)) + 1
          return ((long)(((ulong)(-(a + 1))) / ((ulong)(unchecked(-b))))) + 1;
        }
      }
    }
    public static BigInteger EuclideanDivision(BigInteger a, BigInteger b) {
      if (0 <= a.Sign) {
        if (0 <= b.Sign) {
          // +a +b: a/b
          return BigInteger.Divide(a, b);
        } else {
          // +a -b: -(a/(-b))
          return BigInteger.Negate(BigInteger.Divide(a, BigInteger.Negate(b)));
        }
      } else {
        if (0 <= b.Sign) {
          // -a +b: -((-a-1)/b) - 1
          return BigInteger.Negate(BigInteger.Divide(BigInteger.Negate(a) - 1, b)) - 1;
        } else {
          // -a -b: ((-a-1)/(-b)) + 1
          return BigInteger.Divide(BigInteger.Negate(a) - 1, BigInteger.Negate(b)) + 1;
        }
      }
    }
    // pre: b != 0
    // post: result == a%b, as defined by Euclidean Division (http://en.wikipedia.org/wiki/Modulo_operation)
    public static sbyte EuclideanModulus_sbyte(sbyte a, sbyte b) {
      return (sbyte)EuclideanModulus_int(a, b);
    }
    public static short EuclideanModulus_short(short a, short b) {
      return (short)EuclideanModulus_int(a, b);
    }
    public static int EuclideanModulus_int(int a, int b) {
      uint bp = (0 <= b) ? (uint)b : (uint)(unchecked(-b));
      if (0 <= a) {
        // +a: a % b'
        return (int)(((uint)a) % bp);
      } else {
        // c = ((-a) % b')
        // -a: b' - c if c > 0
        // -a: 0 if c == 0
        uint c = ((uint)(unchecked(-a))) % bp;
        return (int)(c == 0 ? c : bp - c);
      }
    }
    public static long EuclideanModulus_long(long a, long b) {
      ulong bp = (0 <= b) ? (ulong)b : (ulong)(unchecked(-b));
      if (0 <= a) {
        // +a: a % b'
        return (long)(((ulong)a) % bp);
      } else {
        // c = ((-a) % b')
        // -a: b' - c if c > 0
        // -a: 0 if c == 0
        ulong c = ((ulong)(unchecked(-a))) % bp;
        return (long)(c == 0 ? c : bp - c);
      }
    }
    public static BigInteger EuclideanModulus(BigInteger a, BigInteger b) {
      var bp = BigInteger.Abs(b);
      if (0 <= a.Sign) {
        // +a: a % b'
        return BigInteger.Remainder(a, bp);
      } else {
        // c = ((-a) % b')
        // -a: b' - c if c > 0
        // -a: 0 if c == 0
        var c = BigInteger.Remainder(BigInteger.Negate(a), bp);
        return c.IsZero ? c : BigInteger.Subtract(bp, c);
      }
    }
    public static Sequence<T> SeqFromArray<T>(T[] array) {
      return new Sequence<T>((T[])array.Clone());
    }
    // In .NET version 4.5, it it possible to mark a method with "AggressiveInlining", which says to inline the
    // method if possible.  Method "ExpressionSequence" would be a good candidate for it:
    // [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static U ExpressionSequence<T, U>(T t, U u)
    {
      return u;
    }

    public static U Let<T, U>(T t, Func<T,U> f) {
      return f(t);
    }

    public delegate Result Function<Input,Result>(Input input);

    public static A Id<A>(A a) {
      return a;
    }
  }

  public struct BigRational
  {
    public static readonly BigRational ZERO = new BigRational(0);

    BigInteger num, den;  // invariant 1 <= den
    public override string ToString() {
      return string.Format("({0}.0 / {1}.0)", num, den);
    }
    public BigRational(int n) {
      num = new BigInteger(n);
      den = BigInteger.One;
    }
    public BigRational(BigInteger n, BigInteger d) {
      // requires 1 <= d
      num = n;
      den = d;
    }
    public BigInteger ToBigInteger() {
      if (0 <= num) {
        return num / den;
      } else {
        return (num - den + 1) / den;
      }
    }
    /// <summary>
    /// Returns values such that aa/dd == a and bb/dd == b.
    /// </summary>
    private static void Normalize(BigRational a, BigRational b, out BigInteger aa, out BigInteger bb, out BigInteger dd) {
      var gcd = BigInteger.GreatestCommonDivisor(a.den, b.den);
      var xx = a.den / gcd;
      var yy = b.den / gcd;
      // We now have a == a.num / (xx * gcd) and b == b.num / (yy * gcd).
      aa = a.num * yy;
      bb = b.num * xx;
      dd = a.den * yy;
    }
    public int CompareTo(BigRational that) {
      // simple things first
      int asign = this.num.Sign;
      int bsign = that.num.Sign;
      if (asign < 0 && 0 <= bsign) {
        return -1;
      } else if (asign <= 0 && 0 < bsign) {
        return -1;
      } else if (bsign < 0 && 0 <= asign) {
        return 1;
      } else if (bsign <= 0 && 0 < asign) {
        return 1;
      }
      BigInteger aa, bb, dd;
      Normalize(this, that, out aa, out bb, out dd);
      return aa.CompareTo(bb);
    }
    public override int GetHashCode() {
      return num.GetHashCode() + 29 * den.GetHashCode();
    }
    public override bool Equals(object obj) {
      if (obj is BigRational) {
        return this == (BigRational)obj;
      } else {
        return false;
      }
    }
    public static bool operator ==(BigRational a, BigRational b) {
      return a.CompareTo(b) == 0;
    }
    public static bool operator !=(BigRational a, BigRational b) {
      return a.CompareTo(b) != 0;
    }
    public static bool operator >(BigRational a, BigRational b) {
      return 0 < a.CompareTo(b);
    }
    public static bool operator >=(BigRational a, BigRational b) {
      return 0 <= a.CompareTo(b);
    }
    public static bool operator <(BigRational a, BigRational b) {
      return a.CompareTo(b) < 0;
    }
    public static bool operator <=(BigRational a, BigRational b) {
      return a.CompareTo(b) <= 0;
    }
    public static BigRational operator +(BigRational a, BigRational b) {
      BigInteger aa, bb, dd;
      Normalize(a, b, out aa, out bb, out dd);
      return new BigRational(aa + bb, dd);
    }
    public static BigRational operator -(BigRational a, BigRational b) {
      BigInteger aa, bb, dd;
      Normalize(a, b, out aa, out bb, out dd);
      return new BigRational(aa - bb, dd);
    }
    public static BigRational operator -(BigRational a) {
      return new BigRational(-a.num, a.den);
    }
    public static BigRational operator *(BigRational a, BigRational b) {
      return new BigRational(a.num * b.num, a.den * b.den);
    }
    public static BigRational operator /(BigRational a, BigRational b) {
      // Compute the reciprocal of b
      BigRational bReciprocal;
      if (0 < b.num) {
        bReciprocal = new BigRational(b.den, b.num);
      } else {
        // this is the case b.num < 0
        bReciprocal = new BigRational(-b.den, -b.num);
      }
      return a * bReciprocal;
    }
  }
}
namespace Dafny {
  public partial class Helpers {
      public static T[] InitNewArray1<T>(BigInteger size0) {
        int s0 = (int)size0;
        T[] a = new T[s0];
        BigInteger[] b = a as BigInteger[];
        if (b != null) {
          BigInteger z = new BigInteger(0);
          for (int i0 = 0; i0 < s0; i0++)
            b[i0] = z;
        }
        return a;
      }
  }
}
namespace @_System {



  public abstract class Base___tuple_h2<@T0,@T1> { }
  public class __tuple_h2____hMake2<@T0,@T1> : Base___tuple_h2<@T0,@T1> {
    public readonly @T0 @_0;
    public readonly @T1 @_1;
    public __tuple_h2____hMake2(@T0 @_0, @T1 @_1) {
      this.@_0 = @_0;
      this.@_1 = @_1;
    }
    public override bool Equals(object other) {
      var oth = other as _System.@__tuple_h2____hMake2<@T0,@T1>;
      return oth != null && this.@_0.Equals(oth.@_0) && this.@_1.Equals(oth.@_1);
    }
    public override int GetHashCode() {
      ulong hash = 5381;
      hash = ((hash << 5) + hash) + 0;
      hash = ((hash << 5) + hash) + ((ulong)this.@_0.GetHashCode());
      hash = ((hash << 5) + hash) + ((ulong)this.@_1.GetHashCode());
      return (int) hash;
    }
    public override string ToString() {
      string s = "";
      s += "(";
      s += @_0.ToString();
      s += ", ";
      s += @_1.ToString();
      s += ")";
      return s;
    }
  }
  public struct @__tuple_h2<@T0,@T1> {
    Base___tuple_h2<@T0,@T1> _d;
    public Base___tuple_h2<@T0,@T1> _D {
      get {
        if (_d == null) {
          _d = Default;
        }
        return _d;
      }
    }
    public @__tuple_h2(Base___tuple_h2<@T0,@T1> d) { this._d = d; }
    static Base___tuple_h2<@T0,@T1> theDefault;
    public static Base___tuple_h2<@T0,@T1> Default {
      get {
        if (theDefault == null) {
          theDefault = new _System.@__tuple_h2____hMake2<@T0,@T1>(default(@T0), default(@T1));
        }
        return theDefault;
      }
    }
    public override bool Equals(object other) {
      return other is @__tuple_h2<@T0,@T1> && _D.Equals(((@__tuple_h2<@T0,@T1>)other)._D);
    }
    public override int GetHashCode() { return _D.GetHashCode(); }
    public override string ToString() { return _D.ToString(); }
    public bool is____hMake2 { get { return _D is __tuple_h2____hMake2<@T0,@T1>; } }
    public @T0 dtor__0 { get { return ((__tuple_h2____hMake2<@T0,@T1>)_D).@_0; } }
    public @T1 dtor__1 { get { return ((__tuple_h2____hMake2<@T0,@T1>)_D).@_1; } }
  }
} // end of namespace _System

public abstract class Base_Disk { }
public class Disk_White : Base_Disk {
  public Disk_White() {
  }
  public override bool Equals(object other) {
    var oth = other as Disk_White;
    return oth != null;
  }
  public override int GetHashCode() {
    ulong hash = 5381;
    hash = ((hash << 5) + hash) + 0;
    return (int) hash;
  }
  public override string ToString() {
    string s = "Disk.White";
    return s;
  }
}
public class Disk_Black : Base_Disk {
  public Disk_Black() {
  }
  public override bool Equals(object other) {
    var oth = other as Disk_Black;
    return oth != null;
  }
  public override int GetHashCode() {
    ulong hash = 5381;
    hash = ((hash << 5) + hash) + 0;
    return (int) hash;
  }
  public override string ToString() {
    string s = "Disk.Black";
    return s;
  }
}
public struct @Disk {
  Base_Disk _d;
  public Base_Disk _D {
    get {
      if (_d == null) {
        _d = Default;
      }
      return _d;
    }
  }
  public @Disk(Base_Disk d) { this._d = d; }
  static Base_Disk theDefault;
  public static Base_Disk Default {
    get {
      if (theDefault == null) {
        theDefault = new Disk_White();
      }
      return theDefault;
    }
  }
  public override bool Equals(object other) {
    return other is @Disk && _D.Equals(((@Disk)other)._D);
  }
  public override int GetHashCode() { return _D.GetHashCode(); }
  public override string ToString() { return _D.ToString(); }
  public bool is_White { get { return _D is Disk_White; } }
  public bool is_Black { get { return _D is Disk_Black; } }
  public static System.Collections.Generic.IEnumerable<@Disk> AllSingletonConstructors {
    get {
      yield return new @Disk(new Disk_White());
      yield return new @Disk(new Disk_Black());
      yield break;
    }
  }
}



public abstract class Base_Direction { }
public class Direction_Up : Base_Direction {
  public Direction_Up() {
  }
  public override bool Equals(object other) {
    var oth = other as Direction_Up;
    return oth != null;
  }
  public override int GetHashCode() {
    ulong hash = 5381;
    hash = ((hash << 5) + hash) + 0;
    return (int) hash;
  }
  public override string ToString() {
    string s = "Direction.Up";
    return s;
  }
}
public class Direction_UpRight : Base_Direction {
  public Direction_UpRight() {
  }
  public override bool Equals(object other) {
    var oth = other as Direction_UpRight;
    return oth != null;
  }
  public override int GetHashCode() {
    ulong hash = 5381;
    hash = ((hash << 5) + hash) + 0;
    return (int) hash;
  }
  public override string ToString() {
    string s = "Direction.UpRight";
    return s;
  }
}
public class Direction_Right : Base_Direction {
  public Direction_Right() {
  }
  public override bool Equals(object other) {
    var oth = other as Direction_Right;
    return oth != null;
  }
  public override int GetHashCode() {
    ulong hash = 5381;
    hash = ((hash << 5) + hash) + 0;
    return (int) hash;
  }
  public override string ToString() {
    string s = "Direction.Right";
    return s;
  }
}
public class Direction_DownRight : Base_Direction {
  public Direction_DownRight() {
  }
  public override bool Equals(object other) {
    var oth = other as Direction_DownRight;
    return oth != null;
  }
  public override int GetHashCode() {
    ulong hash = 5381;
    hash = ((hash << 5) + hash) + 0;
    return (int) hash;
  }
  public override string ToString() {
    string s = "Direction.DownRight";
    return s;
  }
}
public class Direction_Down : Base_Direction {
  public Direction_Down() {
  }
  public override bool Equals(object other) {
    var oth = other as Direction_Down;
    return oth != null;
  }
  public override int GetHashCode() {
    ulong hash = 5381;
    hash = ((hash << 5) + hash) + 0;
    return (int) hash;
  }
  public override string ToString() {
    string s = "Direction.Down";
    return s;
  }
}
public class Direction_DownLeft : Base_Direction {
  public Direction_DownLeft() {
  }
  public override bool Equals(object other) {
    var oth = other as Direction_DownLeft;
    return oth != null;
  }
  public override int GetHashCode() {
    ulong hash = 5381;
    hash = ((hash << 5) + hash) + 0;
    return (int) hash;
  }
  public override string ToString() {
    string s = "Direction.DownLeft";
    return s;
  }
}
public class Direction_Left : Base_Direction {
  public Direction_Left() {
  }
  public override bool Equals(object other) {
    var oth = other as Direction_Left;
    return oth != null;
  }
  public override int GetHashCode() {
    ulong hash = 5381;
    hash = ((hash << 5) + hash) + 0;
    return (int) hash;
  }
  public override string ToString() {
    string s = "Direction.Left";
    return s;
  }
}
public class Direction_UpLeft : Base_Direction {
  public Direction_UpLeft() {
  }
  public override bool Equals(object other) {
    var oth = other as Direction_UpLeft;
    return oth != null;
  }
  public override int GetHashCode() {
    ulong hash = 5381;
    hash = ((hash << 5) + hash) + 0;
    return (int) hash;
  }
  public override string ToString() {
    string s = "Direction.UpLeft";
    return s;
  }
}
public struct @Direction {
  Base_Direction _d;
  public Base_Direction _D {
    get {
      if (_d == null) {
        _d = Default;
      }
      return _d;
    }
  }
  public @Direction(Base_Direction d) { this._d = d; }
  static Base_Direction theDefault;
  public static Base_Direction Default {
    get {
      if (theDefault == null) {
        theDefault = new Direction_Up();
      }
      return theDefault;
    }
  }
  public override bool Equals(object other) {
    return other is @Direction && _D.Equals(((@Direction)other)._D);
  }
  public override int GetHashCode() { return _D.GetHashCode(); }
  public override string ToString() { return _D.ToString(); }
  public bool is_Up { get { return _D is Direction_Up; } }
  public bool is_UpRight { get { return _D is Direction_UpRight; } }
  public bool is_Right { get { return _D is Direction_Right; } }
  public bool is_DownRight { get { return _D is Direction_DownRight; } }
  public bool is_Down { get { return _D is Direction_Down; } }
  public bool is_DownLeft { get { return _D is Direction_DownLeft; } }
  public bool is_Left { get { return _D is Direction_Left; } }
  public bool is_UpLeft { get { return _D is Direction_UpLeft; } }
  public static System.Collections.Generic.IEnumerable<@Direction> AllSingletonConstructors {
    get {
      yield return new @Direction(new Direction_Up());
      yield return new @Direction(new Direction_UpRight());
      yield return new @Direction(new Direction_Right());
      yield return new @Direction(new Direction_DownRight());
      yield return new @Direction(new Direction_Down());
      yield return new @Direction(new Direction_DownLeft());
      yield return new @Direction(new Direction_Left());
      yield return new @Direction(new Direction_UpLeft());
      yield break;
    }
  }
}

public partial class @__default {
  public static void @Main()
  {
  TAIL_CALL_START: ;
    Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @_1457_board = Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk>.Empty;
    Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> _out0;
    @__default.@InitBoard(out _out0);
    @_1457_board = _out0;
    @Disk @_1458_player = new @Disk();
    @_1458_player = new @Disk(new Disk_Black());
    Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1459_legalMoves = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> _out1;
    @__default.@FindAllLegalMoves(@_1457_board, @_1458_player, out _out1);
    @_1459_legalMoves = _out1;
    { }
    while (!(new BigInteger((@_1459_legalMoves).Length)).@Equals(new BigInteger(0)))
    {
      @_System.@__tuple_h2<BigInteger,BigInteger> @_1460_move = new @_System.@__tuple_h2<BigInteger,BigInteger>();
      if ((@_1458_player).@Equals(new @Disk(new Disk_Black())))
      {
        { }
        { }
        { }
        @_System.@__tuple_h2<BigInteger,BigInteger> _out2;
        @__default.@SelectBlackMove(@_1457_board, @_1459_legalMoves, out _out2);
        @_1460_move = _out2;
      }
      else
      {
        { }
        { }
        { }
        @_System.@__tuple_h2<BigInteger,BigInteger> _out3;
        @__default.@SelectWhiteMove(@_1457_board, @_1459_legalMoves, out _out3);
        @_1460_move = _out3;
      }
      @__default.@PrintMoveDetails(@_1457_board, @_1458_player, @_1460_move);
      Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> _out4;
      @__default.@PerformMove(@_1457_board, @_1458_player, @_1460_move, out _out4);
      @_1457_board = _out4;
      @_1458_player = ((@_1458_player).@Equals(new @Disk(new Disk_Black()))) ? (new @Disk(new Disk_White())) : (new @Disk(new Disk_Black()));
      Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> _out5;
      @__default.@FindAllLegalMoves(@_1457_board, @_1458_player, out _out5);
      @_1459_legalMoves = _out5;
      if ((new BigInteger((@_1459_legalMoves).Length)).@Equals(new BigInteger(0)))
      {
        @_1458_player = ((@_1458_player).@Equals(new @Disk(new Disk_White()))) ? (new @Disk(new Disk_Black())) : (new @Disk(new Disk_White()));
        Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> _out6;
        @__default.@FindAllLegalMoves(@_1457_board, @_1458_player, out _out6);
        @_1459_legalMoves = _out6;
      }
    }
    { }
    BigInteger @_1461_blacks = BigInteger.Zero;
    BigInteger @_1462_whites = BigInteger.Zero;
    BigInteger _out7;
    BigInteger _out8;
    @__default.@TotalScore(@_1457_board, out _out7, out _out8);
    @_1461_blacks = _out7;
    @_1462_whites = _out8;
    @__default.@PrintResults(@_1461_blacks, @_1462_whites);
  }
  public static void @PrintMoveDetails(Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @board, @Disk @player, @_System.@__tuple_h2<BigInteger,BigInteger> @move)
  {
  TAIL_CALL_START: ;
    System.Console.Write(Dafny.Sequence<char>.FromString("\n"));
    System.Console.Write(@player);
    System.Console.Write(Dafny.Sequence<char>.FromString(" is placed on row "));
    System.Console.Write((@move).@dtor__0);
    System.Console.Write(Dafny.Sequence<char>.FromString(" and column "));
    System.Console.Write((@move).@dtor__1);
    Dafny.Set<@Direction> @_1463_reversibleDirections = Dafny.Set<@Direction>.Empty;
    Dafny.Set<@Direction> _out9;
    @__default.@FindAllLegalDirectionsFrom(@board, @player, @move, out _out9);
    @_1463_reversibleDirections = _out9;
    System.Console.Write(Dafny.Sequence<char>.FromString(" with reversible directions "));
    System.Console.Write(@_1463_reversibleDirections);
    Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1464_reversiblePositions = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> _out10;
    @__default.@FindAllReversiblePositionsFrom(@board, @player, @move, out _out10);
    @_1464_reversiblePositions = _out10;
    System.Console.Write(Dafny.Sequence<char>.FromString(" and reversible positions "));
    System.Console.Write(@_1464_reversiblePositions);
  }
  public static void @PrintResults(BigInteger @blacks, BigInteger @whites)
  {
  TAIL_CALL_START: ;
    System.Console.Write(Dafny.Sequence<char>.FromString("\nGame Over.\nAnd the winner is... "));
    System.Console.Write(((@blacks) > (@whites)) ? (Dafny.Sequence<char>.FromString("The Black.")) : (((@blacks) < (@whites)) ? (Dafny.Sequence<char>.FromString("The White.")) : (Dafny.Sequence<char>.FromString("it's a tie."))));
    System.Console.Write(Dafny.Sequence<char>.FromString("\nFinal Score: "));
    System.Console.Write(@blacks);
    System.Console.Write(Dafny.Sequence<char>.FromString(" Black disks versus "));
    System.Console.Write(@whites);
    System.Console.Write(Dafny.Sequence<char>.FromString(" White disks."));
  }
  public static Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @ValidPositions() {
    return Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(0), new BigInteger(0))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(0), new BigInteger(1))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(0), new BigInteger(2))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(0), new BigInteger(3))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(0), new BigInteger(4))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(0), new BigInteger(5))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(0), new BigInteger(6))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(0), new BigInteger(7))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(1), new BigInteger(0))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(1), new BigInteger(1))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(1), new BigInteger(2))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(1), new BigInteger(3))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(1), new BigInteger(4))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(1), new BigInteger(5))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(1), new BigInteger(6))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(1), new BigInteger(7))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(2), new BigInteger(0))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(2), new BigInteger(1))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(2), new BigInteger(2))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(2), new BigInteger(3))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(2), new BigInteger(4))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(2), new BigInteger(5))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(2), new BigInteger(6))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(2), new BigInteger(7))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(3), new BigInteger(0))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(3), new BigInteger(1))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(3), new BigInteger(2))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(3), new BigInteger(3))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(3), new BigInteger(4))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(3), new BigInteger(5))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(3), new BigInteger(6))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(3), new BigInteger(7))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(4), new BigInteger(0))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(4), new BigInteger(1))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(4), new BigInteger(2))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(4), new BigInteger(3))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(4), new BigInteger(4))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(4), new BigInteger(5))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(4), new BigInteger(6))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(4), new BigInteger(7))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(5), new BigInteger(0))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(5), new BigInteger(1))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(5), new BigInteger(2))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(5), new BigInteger(3))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(5), new BigInteger(4))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(5), new BigInteger(5))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(5), new BigInteger(6))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(5), new BigInteger(7))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(6), new BigInteger(0))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(6), new BigInteger(1))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(6), new BigInteger(2))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(6), new BigInteger(3))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(6), new BigInteger(4))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(6), new BigInteger(5))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(6), new BigInteger(6))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(6), new BigInteger(7))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(7), new BigInteger(0))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(7), new BigInteger(1))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(7), new BigInteger(2))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(7), new BigInteger(3))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(7), new BigInteger(4))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(7), new BigInteger(5))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(7), new BigInteger(6))), new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(7), new BigInteger(7))));
  }
  public static void @OccupiedByM(Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @b, @_System.@__tuple_h2<BigInteger,BigInteger> @pos, @Disk @player, out bool @ans)
  {
    @ans = false;
  TAIL_CALL_START: ;
    @ans = ((((@__default.@ValidPositions()).@Contains(@pos)) && ((@b).@Contains(@pos))) && (((@b).Select(@pos)).@Equals(@player))) ? (true) : (false);
  }
  public static void @AvailablePositionsM(Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @b, out Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @res)
  {
    @res = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
  TAIL_CALL_START: ;
    @res = ((Dafny.Helpers.ComprehensionDelegate<@_System.@__tuple_h2<BigInteger,BigInteger>>)delegate() { var _coll0 = new System.Collections.Generic.List<@_System.@__tuple_h2<BigInteger,BigInteger>>(); foreach (var @_1465_pos in (@__default.@ValidPositions()).Elements) { if ((((@__default.@ValidPositions()).@Contains(@_1465_pos)) && ((@__default.@ValidPositions()).@Contains(@_1465_pos))) && (!(@b).@Contains(@_1465_pos))) {_coll0.Add(@_1465_pos); }}return Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromCollection(_coll0); })();
  }
  public static void @PlayerPositionsM(Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @b, @Disk @player, out Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @res)
  {
    @res = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
  TAIL_CALL_START: ;
    @res = ((Dafny.Helpers.ComprehensionDelegate<@_System.@__tuple_h2<BigInteger,BigInteger>>)delegate() { var _coll1 = new System.Collections.Generic.List<@_System.@__tuple_h2<BigInteger,BigInteger>>(); foreach (var @_1466_pos in (@__default.@ValidPositions()).Elements) { if ((((@__default.@ValidPositions()).@Contains(@_1466_pos)) && ((@b).@Contains(@_1466_pos))) && (((@b).Select(@_1466_pos)).@Equals(@player))) {_coll1.Add(@_1466_pos); }}return Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromCollection(_coll1); })();
  }
  public static void @CountM(Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @b, @Disk @player, out BigInteger @res)
  {
    @res = BigInteger.Zero;
  TAIL_CALL_START: ;
    Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1467_playerPos = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> _out11;
    @__default.@PlayerPositionsM(@b, @player, out _out11);
    @_1467_playerPos = _out11;
    @res = new BigInteger((@_1467_playerPos).Length);
  }
  public static void @LegalMoveM(Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @b, @Disk @player, @_System.@__tuple_h2<BigInteger,BigInteger> @pos, out bool @ans)
  {
    @ans = false;
  TAIL_CALL_START: ;
    @ans = false;
    bool @_1468_ans1 = false;
    @_1468_ans1 = ((@__default.@ValidPositions()).@Contains(@pos)) && (!(@b).@Contains(@pos));
    { }
    @Direction @_1469_ensuredValidDir = new @Direction();
    if (!(@_1468_ans1))
    {
      @ans = false;
      { }
    }
    else
    {
      { }
      { }
      Dafny.Set<@Direction> @_1470_dirs = Dafny.Set<@Direction>.Empty;
      @_1470_dirs = Dafny.Set<@Direction>.FromElements(new @Direction(new Direction_Up()), new @Direction(new Direction_UpRight()), new @Direction(new Direction_Right()), new @Direction(new Direction_DownRight()), new @Direction(new Direction_Down()), new @Direction(new Direction_DownLeft()), new @Direction(new Direction_Left()), new @Direction(new Direction_UpLeft()));
      Dafny.Set<@Direction> @_1471_checkedDirs = Dafny.Set<@Direction>.Empty;
      @_1471_checkedDirs = Dafny.Set<@Direction>.FromElements();
      @Direction @_1472_dir = new @Direction();
      bool @_1473_VD = false;
      while ((new BigInteger((@_1470_dirs).Length)) > (new BigInteger(0)))
      {
        foreach (var _assign_such_that_0 in (@_1470_dirs).Elements) { @_1472_dir = _assign_such_that_0;
          if ((@_1470_dirs).@Contains(@_1472_dir)) {
            goto _ASSIGN_SUCH_THAT_0;
          }
        }
        throw new System.Exception("assign-such-that search produced no value (line 399)");
        _ASSIGN_SUCH_THAT_0: ;
        { }
        bool _out12;
        @__default.@ValidDirectionFrom(@b, @_1472_dir, @pos, @player, out _out12);
        @_1473_VD = _out12;
        { }
        if (@_1473_VD)
        {
          { }
          @ans = true;
          @_1469_ensuredValidDir = @_1472_dir;
          { }
          { }
          { }
        }
        @_1471_checkedDirs = (@_1471_checkedDirs).@Union(Dafny.Set<@Direction>.FromElements(@_1472_dir));
        @_1470_dirs = (@_1470_dirs).@Difference(Dafny.Set<@Direction>.FromElements(@_1472_dir));
        { }
        { }
        { }
        { }
        { }
      }
      { }
      { }
      { }
      { }
      { }
      { }
      { }
      { }
    }
    { }
    { }
  }
  public static void @OpponentM(@Disk @player, out @Disk @p)
  {
    @p = new @Disk();
  TAIL_CALL_START: ;
    if ((@player).@Equals(new @Disk(new Disk_White())))
    {
      @p = new @Disk(new Disk_Black());
    }
    else
    {
      @p = new @Disk(new Disk_White());
    }
  }
  public static void @SelectBlackMove(Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @b, Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @moves, out @_System.@__tuple_h2<BigInteger,BigInteger> @pos)
  {
    @pos = new @_System.@__tuple_h2<BigInteger,BigInteger>();
  TAIL_CALL_START: ;
    foreach (var _assign_such_that_1 in (@moves).Elements) { @pos = _assign_such_that_1;
      if ((@moves).@Contains(@pos)) {
        goto _ASSIGN_SUCH_THAT_1;
      }
    }
    throw new System.Exception("assign-such-that search produced no value (line 464)");
    _ASSIGN_SUCH_THAT_1: ;
  }
  public static void @SelectWhiteMove(Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @b, Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @moves, out @_System.@__tuple_h2<BigInteger,BigInteger> @pos)
  {
    @pos = new @_System.@__tuple_h2<BigInteger,BigInteger>();
  TAIL_CALL_START: ;
    foreach (var _assign_such_that_2 in (@moves).Elements) { @pos = _assign_such_that_2;
      if ((@moves).@Contains(@pos)) {
        goto _ASSIGN_SUCH_THAT_2;
      }
    }
    throw new System.Exception("assign-such-that search produced no value (line 473)");
    _ASSIGN_SUCH_THAT_2: ;
  }
  public static void @InitBoard(out Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @b)
  {
    @b = Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk>.Empty;
  TAIL_CALL_START: ;
    @b = Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk>.FromElements(new Dafny.Pair<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk>(new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(3), new BigInteger(3))),new @Disk(new Disk_White())), new Dafny.Pair<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk>(new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(3), new BigInteger(4))),new @Disk(new Disk_Black())), new Dafny.Pair<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk>(new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(4), new BigInteger(3))),new @Disk(new Disk_Black())), new Dafny.Pair<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk>(new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(4), new BigInteger(4))),new @Disk(new Disk_White())));
  }
  public static void @PlayerPositionsMethod(Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @b, @Disk @player, out Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @PlayerPosSet)
  {
    @PlayerPosSet = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
  TAIL_CALL_START: ;
    @PlayerPosSet = ((Dafny.Helpers.ComprehensionDelegate<@_System.@__tuple_h2<BigInteger,BigInteger>>)delegate() { var _coll2 = new System.Collections.Generic.List<@_System.@__tuple_h2<BigInteger,BigInteger>>(); foreach (var @_1474_pos in (@__default.@ValidPositions()).Elements) { if ((((@__default.@ValidPositions()).@Contains(@_1474_pos)) && ((@b).@Contains(@_1474_pos))) && (((@b).Select(@_1474_pos)).@Equals(@player))) {_coll2.Add(@_1474_pos); }}return Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromCollection(_coll2); })();
  }
  public static void @VectorM(@Direction @d, @_System.@__tuple_h2<BigInteger,BigInteger> @pos, out Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @res)
  {
    @res = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
  TAIL_CALL_START: ;
    @Direction _source0 = @d;
    if (_source0.is_Up) {
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out13;
      @__default.@VectorUp(@pos, out _out13);
      @res = _out13;
    } else if (_source0.is_UpRight) {
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out14;
      @__default.@VectorUpRight(@pos, out _out14);
      @res = _out14;
    } else if (_source0.is_Right) {
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out15;
      @__default.@VectorRight(@pos, out _out15);
      @res = _out15;
    } else if (_source0.is_DownRight) {
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out16;
      @__default.@VectorDownRight(@pos, out _out16);
      @res = _out16;
    } else if (_source0.is_Down) {
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out17;
      @__default.@VectorDown(@pos, out _out17);
      @res = _out17;
    } else if (_source0.is_DownLeft) {
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out18;
      @__default.@VectorDownLeft(@pos, out _out18);
      @res = _out18;
    } else if (_source0.is_Left) {
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out19;
      @__default.@VectorLeft(@pos, out _out19);
      @res = _out19;
    } else if (true) {
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out20;
      @__default.@VectorUpLeft(@pos, out _out20);
      @res = _out20;
    }
  }
  public static void @VectorUp(@_System.@__tuple_h2<BigInteger,BigInteger> @pos, out Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @res)
  {
    @res = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    if (((@pos).@dtor__0).@Equals(new BigInteger(0)))
    {
      @res = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@pos);
    }
    else
    {
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1475_recursiveCall = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out21;
      @__default.@VectorUp(new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(((@pos).@dtor__0) - (new BigInteger(1)), (@pos).@dtor__1)), out _out21);
      @_1475_recursiveCall = _out21;
      @res = (Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@pos)).@Concat(@_1475_recursiveCall);
    }
  }
  public static void @VectorUpRight(@_System.@__tuple_h2<BigInteger,BigInteger> @pos, out Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @res)
  {
    @res = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    if ((((@pos).@dtor__0).@Equals(new BigInteger(0))) || (((@pos).@dtor__1).@Equals(new BigInteger(7))))
    {
      @res = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@pos);
    }
    else
    {
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1476_recursiveCall = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out22;
      @__default.@VectorUpRight(new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(((@pos).@dtor__0) - (new BigInteger(1)), ((@pos).@dtor__1) + (new BigInteger(1)))), out _out22);
      @_1476_recursiveCall = _out22;
      @res = (Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@pos)).@Concat(@_1476_recursiveCall);
    }
  }
  public static void @VectorUpLeft(@_System.@__tuple_h2<BigInteger,BigInteger> @pos, out Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @res)
  {
    @res = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    if ((((@pos).@dtor__0).@Equals(new BigInteger(0))) || (((@pos).@dtor__1).@Equals(new BigInteger(0))))
    {
      @res = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@pos);
    }
    else
    {
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1477_recursiveCall = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out23;
      @__default.@VectorUpLeft(new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(((@pos).@dtor__0) - (new BigInteger(1)), ((@pos).@dtor__1) - (new BigInteger(1)))), out _out23);
      @_1477_recursiveCall = _out23;
      @res = (Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@pos)).@Concat(@_1477_recursiveCall);
    }
  }
  public static void @VectorRight(@_System.@__tuple_h2<BigInteger,BigInteger> @pos, out Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @res)
  {
    @res = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    if (((@pos).@dtor__1).@Equals(new BigInteger(7)))
    {
      @res = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@pos);
    }
    else
    {
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1478_recursiveCall = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out24;
      @__default.@VectorRight(new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>((@pos).@dtor__0, ((@pos).@dtor__1) + (new BigInteger(1)))), out _out24);
      @_1478_recursiveCall = _out24;
      @res = (Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@pos)).@Concat(@_1478_recursiveCall);
    }
  }
  public static void @VectorLeft(@_System.@__tuple_h2<BigInteger,BigInteger> @pos, out Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @res)
  {
    @res = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    if (((@pos).@dtor__1).@Equals(new BigInteger(0)))
    {
      @res = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@pos);
    }
    else
    {
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1479_recursiveCall = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out25;
      @__default.@VectorLeft(new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>((@pos).@dtor__0, ((@pos).@dtor__1) - (new BigInteger(1)))), out _out25);
      @_1479_recursiveCall = _out25;
      @res = (Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@pos)).@Concat(@_1479_recursiveCall);
    }
  }
  public static void @VectorDown(@_System.@__tuple_h2<BigInteger,BigInteger> @pos, out Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @res)
  {
    @res = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    if (((@pos).@dtor__0).@Equals(new BigInteger(7)))
    {
      @res = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@pos);
    }
    else
    {
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1480_recursiveCall = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out26;
      @__default.@VectorDown(new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(((@pos).@dtor__0) + (new BigInteger(1)), (@pos).@dtor__1)), out _out26);
      @_1480_recursiveCall = _out26;
      @res = (Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@pos)).@Concat(@_1480_recursiveCall);
    }
  }
  public static void @VectorDownRight(@_System.@__tuple_h2<BigInteger,BigInteger> @pos, out Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @res)
  {
    @res = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    if ((((@pos).@dtor__0).@Equals(new BigInteger(7))) || (((@pos).@dtor__1).@Equals(new BigInteger(7))))
    {
      @res = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@pos);
    }
    else
    {
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1481_recursiveCall = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out27;
      @__default.@VectorDownRight(new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(((@pos).@dtor__0) + (new BigInteger(1)), ((@pos).@dtor__1) + (new BigInteger(1)))), out _out27);
      @_1481_recursiveCall = _out27;
      @res = (Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@pos)).@Concat(@_1481_recursiveCall);
    }
  }
  public static void @VectorDownLeft(@_System.@__tuple_h2<BigInteger,BigInteger> @pos, out Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @res)
  {
    @res = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    if ((((@pos).@dtor__0).@Equals(new BigInteger(7))) || (((@pos).@dtor__1).@Equals(new BigInteger(0))))
    {
      @res = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@pos);
    }
    else
    {
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1482_recursiveCall = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
      Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out28;
      @__default.@VectorDownLeft(new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(((@pos).@dtor__0) + (new BigInteger(1)), ((@pos).@dtor__1) - (new BigInteger(1)))), out _out28);
      @_1482_recursiveCall = _out28;
      @res = (Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@pos)).@Concat(@_1482_recursiveCall);
    }
  }
  public static void @ReversibleVectorOpponentPositions(Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @b, @Direction @d, @_System.@__tuple_h2<BigInteger,BigInteger> @pos, @Disk @player, out Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @res)
  {
    @res = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
  TAIL_CALL_START: ;
    @_System.@__tuple_h2<BigInteger,BigInteger> @_1483_dummyPosition = new @_System.@__tuple_h2<BigInteger,BigInteger>();
    @_1483_dummyPosition = new @_System.@__tuple_h2<BigInteger,BigInteger>(new _System.@__tuple_h2____hMake2<BigInteger,BigInteger>(new BigInteger(0), new BigInteger(0)));
    Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1484_vector = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out29;
    @__default.@VectorM(@d, @pos, out _out29);
    @_1484_vector = _out29;
    Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1485_positionsVector = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    @_1485_positionsVector = (@_1484_vector).@Concat(Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@_1483_dummyPosition, @_1483_dummyPosition, @_1483_dummyPosition, @_1483_dummyPosition, @_1483_dummyPosition));
    bool @_1486_pV2 = false;
    bool _out30;
    @__default.@OccupiedByM(@b, (@_1485_positionsVector).Select(new BigInteger(2)), @player, out _out30);
    @_1486_pV2 = _out30;
    bool @_1487_pV3 = false;
    bool _out31;
    @__default.@OccupiedByM(@b, (@_1485_positionsVector).Select(new BigInteger(3)), @player, out _out31);
    @_1487_pV3 = _out31;
    bool @_1488_pV4 = false;
    bool _out32;
    @__default.@OccupiedByM(@b, (@_1485_positionsVector).Select(new BigInteger(4)), @player, out _out32);
    @_1488_pV4 = _out32;
    bool @_1489_pV5 = false;
    bool _out33;
    @__default.@OccupiedByM(@b, (@_1485_positionsVector).Select(new BigInteger(5)), @player, out _out33);
    @_1489_pV5 = _out33;
    bool @_1490_pV6 = false;
    bool _out34;
    @__default.@OccupiedByM(@b, (@_1485_positionsVector).Select(new BigInteger(6)), @player, out _out34);
    @_1490_pV6 = _out34;
    BigInteger @_1491_firstCurrentPlayerDiskDistance = BigInteger.Zero;
    @_1491_firstCurrentPlayerDiskDistance = (@_1486_pV2) ? (new BigInteger(2)) : ((@_1487_pV3) ? (new BigInteger(3)) : ((@_1488_pV4) ? (new BigInteger(4)) : ((@_1489_pV5) ? (new BigInteger(5)) : ((@_1490_pV6) ? (new BigInteger(6)) : (new BigInteger(7))))));
    @res = ((Dafny.Helpers.ComprehensionDelegate<@_System.@__tuple_h2<BigInteger,BigInteger>>)delegate() { var _coll3 = new System.Collections.Generic.List<@_System.@__tuple_h2<BigInteger,BigInteger>>(); foreach (var @_1492_pos in ((@_1485_positionsVector).Take(@_1491_firstCurrentPlayerDiskDistance).Drop(new BigInteger(1))).Elements) { if (((@_1485_positionsVector).Take(@_1491_firstCurrentPlayerDiskDistance).Drop(new BigInteger(1))).@Contains(@_1492_pos)) {_coll3.Add(@_1492_pos); }}return Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromCollection(_coll3); })();
  }
  public static void @ValidDirectionFrom(Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @b, @Direction @d, @_System.@__tuple_h2<BigInteger,BigInteger> @pos, @Disk @player, out bool @ans)
  {
    @ans = false;
  TAIL_CALL_START: ;
    Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1493_vector = Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    Dafny.Sequence<@_System.@__tuple_h2<BigInteger,BigInteger>> _out35;
    @__default.@VectorM(@d, @pos, out _out35);
    @_1493_vector = _out35;
    @Disk @_1494_Opp = new @Disk();
    @Disk _out36;
    @__default.@OpponentM(@player, out _out36);
    @_1494_Opp = _out36;
    @ans = (((new BigInteger((@_1493_vector).Length)) >= (new BigInteger(3))) && (!(@b).@Contains((@_1493_vector).Select(new BigInteger(0))))) && (Dafny.Helpers.QuantInt(new BigInteger(0), new BigInteger((@_1493_vector).Length), false, (@_1495_j => (((((new BigInteger(1)) < (@_1495_j)) && ((@_1495_j) < (new BigInteger((@_1493_vector).Length)))) && ((@b).@Contains((@_1493_vector).Select(@_1495_j)))) && (((@b).Select((@_1493_vector).Select(@_1495_j))).@Equals(@player))) && (Dafny.Helpers.QuantInt((new BigInteger(0)) + (new BigInteger(1)), @_1495_j, true, (@_1496_i => !(((new BigInteger(0)) < (@_1496_i)) && ((@_1496_i) < (@_1495_j))) || (((@b).@Contains((@_1493_vector).Select(@_1496_i))) && (((@b).Select((@_1493_vector).Select(@_1496_i))).@Equals(@_1494_Opp)))))))));
  }
  public static void @TotalScore(Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @b, out BigInteger @blacks, out BigInteger @whites)
  {
    @blacks = BigInteger.Zero;
    @whites = BigInteger.Zero;
  TAIL_CALL_START: ;
    Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1497_AllPositions = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    @_1497_AllPositions = ((Dafny.Helpers.ComprehensionDelegate<@_System.@__tuple_h2<BigInteger,BigInteger>>)delegate() { var _coll4 = new System.Collections.Generic.List<@_System.@__tuple_h2<BigInteger,BigInteger>>(); foreach (var @_1498_pos in (@b).Domain) { if ((@b).@Contains(@_1498_pos)) {_coll4.Add(@_1498_pos); }}return Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromCollection(_coll4); })();
    Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1499_Whites = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    @_1499_Whites = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements();
    Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1500_Blacks = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    @_1500_Blacks = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements();
    @blacks = new BigInteger(0);
    @whites = new BigInteger(0);
    @_System.@__tuple_h2<BigInteger,BigInteger> @_1501_move = new @_System.@__tuple_h2<BigInteger,BigInteger>();
    while ((new BigInteger((@_1497_AllPositions).Length)) > (new BigInteger(0)))
    {
      foreach (var _assign_such_that_3 in (@_1497_AllPositions).Elements) { @_1501_move = _assign_such_that_3;
        if ((@_1497_AllPositions).@Contains(@_1501_move)) {
          goto _ASSIGN_SUCH_THAT_3;
        }
      }
      throw new System.Exception("assign-such-that search produced no value (line 687)");
      _ASSIGN_SUCH_THAT_3: ;
      { }
      if (((@b).Select(@_1501_move)).@Equals(new @Disk(new Disk_Black())))
      {
        { }
        { }
        @blacks = (@blacks) + (new BigInteger(1));
        @_1500_Blacks = (@_1500_Blacks).@Union(Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@_1501_move));
        { }
      }
      else
      {
        { }
        { }
        { }
        @whites = (@whites) + (new BigInteger(1));
        @_1499_Whites = (@_1499_Whites).@Union(Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@_1501_move));
        { }
      }
      { }
      { }
      { }
      @_1497_AllPositions = (@_1497_AllPositions).@Difference(Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@_1501_move));
      { }
    }
    { }
    { }
    { }
    { }
    { }
  }
  public static void @FindAllLegalDirectionsFrom(Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @b, @Disk @player, @_System.@__tuple_h2<BigInteger,BigInteger> @move, out Dafny.Set<@Direction> @directions)
  {
    @directions = Dafny.Set<@Direction>.Empty;
  TAIL_CALL_START: ;
    Dafny.Set<@Direction> @_1502_dirs = Dafny.Set<@Direction>.Empty;
    @_1502_dirs = Dafny.Set<@Direction>.FromElements(new @Direction(new Direction_Up()), new @Direction(new Direction_UpRight()), new @Direction(new Direction_Right()), new @Direction(new Direction_DownRight()), new @Direction(new Direction_Down()), new @Direction(new Direction_DownLeft()), new @Direction(new Direction_Left()), new @Direction(new Direction_UpLeft()));
    @directions = Dafny.Set<@Direction>.FromElements();
    Dafny.Set<@Direction> @_1503_NotValidDirections = Dafny.Set<@Direction>.Empty;
    @_1503_NotValidDirections = Dafny.Set<@Direction>.FromElements();
    bool @_1504_VD = false;
    { }
    { }
    { }
    { }
    while ((new BigInteger((@_1502_dirs).Length)) > (new BigInteger(0)))
    {
      { }
      { }
      { }
      { }
      { }
      @Direction @_1505_dir = new @Direction();
      foreach (var _assign_such_that_4 in (@_1502_dirs).Elements) { @_1505_dir = _assign_such_that_4;
        if ((@_1502_dirs).@Contains(@_1505_dir)) {
          goto _ASSIGN_SUCH_THAT_4;
        }
      }
      throw new System.Exception("assign-such-that search produced no value (line 774)");
      _ASSIGN_SUCH_THAT_4: ;
      { }
      { }
      bool _out37;
      @__default.@ValidDirectionFrom(@b, @_1505_dir, @move, @player, out _out37);
      @_1504_VD = _out37;
      if (@_1504_VD)
      {
        { }
        { }
        @directions = (@directions).@Union(Dafny.Set<@Direction>.FromElements(@_1505_dir));
        { }
        { }
      }
      else
      {
        { }
        { }
        { }
        @_1503_NotValidDirections = (@_1503_NotValidDirections).@Union(Dafny.Set<@Direction>.FromElements(@_1505_dir));
        { }
        { }
      }
      { }
      { }
      { }
      @_1502_dirs = (@_1502_dirs).@Difference(Dafny.Set<@Direction>.FromElements(@_1505_dir));
      { }
      { }
      { }
      { }
    }
    { }
  }
  public static void @FindAllReversiblePositionsFrom(Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @b, @Disk @player, @_System.@__tuple_h2<BigInteger,BigInteger> @move, out Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @positions)
  {
    @positions = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
  TAIL_CALL_START: ;
    { }
    Dafny.Set<@Direction> @_1506_AllDirections = Dafny.Set<@Direction>.Empty;
    Dafny.Set<@Direction> _out38;
    @__default.@FindAllLegalDirectionsFrom(@b, @player, @move, out _out38);
    @_1506_AllDirections = _out38;
    { }
    @positions = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements();
    Dafny.Set<@Direction> @_1507_CheckedDirs = Dafny.Set<@Direction>.Empty;
    @_1507_CheckedDirs = Dafny.Set<@Direction>.FromElements();
    while ((new BigInteger((@_1506_AllDirections).Length)) > (new BigInteger(0)))
    {
      @Direction @_1508_d = new @Direction();
      foreach (var _assign_such_that_5 in (@_1506_AllDirections).Elements) { @_1508_d = _assign_such_that_5;
        if ((@_1506_AllDirections).@Contains(@_1508_d)) {
          goto _ASSIGN_SUCH_THAT_5;
        }
      }
      throw new System.Exception("assign-such-that search produced no value (line 844)");
      _ASSIGN_SUCH_THAT_5: ;
      Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1509_thisDirectionPositions = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
      Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> _out39;
      @__default.@ReversibleVectorOpponentPositions(@b, @_1508_d, @move, @player, out _out39);
      @_1509_thisDirectionPositions = _out39;
      { }
      { }
      { }
      @positions = (@positions).@Union(@_1509_thisDirectionPositions);
      { }
      @_1506_AllDirections = (@_1506_AllDirections).@Difference(Dafny.Set<@Direction>.FromElements(@_1508_d));
      @_1507_CheckedDirs = (@_1507_CheckedDirs).@Union(Dafny.Set<@Direction>.FromElements(@_1508_d));
    }
    { }
  }
  public static void @FindAllLegalMoves(Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @b, @Disk @player, out Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @moves)
  {
    @moves = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
  TAIL_CALL_START: ;
    Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1510_AvailablePos = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> _out40;
    @__default.@AvailablePositionsM(@b, out _out40);
    @_1510_AvailablePos = _out40;
    { }
    { }
    @moves = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements();
    Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1511_IllegalMoves = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    @_1511_IllegalMoves = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements();
    { }
    { }
    bool @_1512_IsLM = false;
    @_System.@__tuple_h2<BigInteger,BigInteger> @_1513_move = new @_System.@__tuple_h2<BigInteger,BigInteger>();
    while ((new BigInteger((@_1510_AvailablePos).Length)) > (new BigInteger(0)))
    {
      foreach (var _assign_such_that_6 in (@_1510_AvailablePos).Elements) { @_1513_move = _assign_such_that_6;
        if ((@_1510_AvailablePos).@Contains(@_1513_move)) {
          goto _ASSIGN_SUCH_THAT_6;
        }
      }
      throw new System.Exception("assign-such-that search produced no value (line 899)");
      _ASSIGN_SUCH_THAT_6: ;
      { }
      bool _out41;
      @__default.@LegalMoveM(@b, @player, @_1513_move, out _out41);
      @_1512_IsLM = _out41;
      if (@_1512_IsLM)
      {
        { }
        { }
        { }
        { }
        { }
        @moves = (@moves).@Union(Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@_1513_move));
        { }
        { }
      }
      else
      {
        { }
        { }
        { }
        { }
        { }
        { }
        { }
        { }
        @_1511_IllegalMoves = (@_1511_IllegalMoves).@Union(Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@_1513_move));
        { }
        { }
        { }
      }
      { }
      { }
      { }
      { }
      @_1510_AvailablePos = (@_1510_AvailablePos).@Difference(Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@_1513_move));
      { }
      { }
    }
  }
  public static void @PerformMove(Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @b0, @Disk @player, @_System.@__tuple_h2<BigInteger,BigInteger> @move, out Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk> @b)
  {
    @b = Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk>.Empty;
  TAIL_CALL_START: ;
    @b = Dafny.Map<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk>.FromElements(new Dafny.Pair<@_System.@__tuple_h2<BigInteger,BigInteger>,@Disk>(@move,@player));
    { }
    Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1514_AllReversiblePos = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> _out42;
    @__default.@FindAllReversiblePositionsFrom(@b0, @player, @move, out _out42);
    @_1514_AllReversiblePos = _out42;
    Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1515_domainb0 = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    @_1515_domainb0 = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements();
    @_1515_domainb0 = ((Dafny.Helpers.ComprehensionDelegate<@_System.@__tuple_h2<BigInteger,BigInteger>>)delegate() { var _coll5 = new System.Collections.Generic.List<@_System.@__tuple_h2<BigInteger,BigInteger>>(); foreach (var @_1516_pos in (@b0).Domain) { if ((@b0).@Contains(@_1516_pos)) {_coll5.Add(@_1516_pos); }}return Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromCollection(_coll5); })();
    { }
    { }
    Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>> @_1517_domainb = Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.Empty;
    @_1517_domainb = @_1515_domainb0;
    { }
    { }
    while ((new BigInteger((@_1517_domainb).Length)) > (new BigInteger(0)))
    {
      { }
      @_System.@__tuple_h2<BigInteger,BigInteger> @_1518_pos = new @_System.@__tuple_h2<BigInteger,BigInteger>();
      foreach (var _assign_such_that_7 in (@_1517_domainb).Elements) { @_1518_pos = _assign_such_that_7;
        if ((@_1517_domainb).@Contains(@_1518_pos)) {
          goto _ASSIGN_SUCH_THAT_7;
        }
      }
      throw new System.Exception("assign-such-that search produced no value (line 996)");
      _ASSIGN_SUCH_THAT_7: ;
      { }
      if (!(@b).@Contains(@_1518_pos))
      {
        if ((@_1514_AllReversiblePos).@Contains(@_1518_pos))
        {
          { }
          @b = (@b).Update(@_1518_pos, @player);
          { }
          { }
          { }
          { }
        }
        else
        {
          { }
          @Disk @_1519_p = new @Disk();
          @_1519_p = (@b0).Select(@_1518_pos);
          @b = (@b).Update(@_1518_pos, @_1519_p);
          { }
          { }
          { }
          { }
          { }
        }
      }
      { }
      @_1517_domainb = (@_1517_domainb).@Difference(Dafny.Set<@_System.@__tuple_h2<BigInteger,BigInteger>>.FromElements(@_1518_pos));
      { }
      { }
      { }
      { }
      { }
      { }
      { }
      { }
    }
    { }
    { }
    { }
    { }
    { }
    { }
    { }
    { }
    { }
  }
}
