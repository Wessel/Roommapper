namespace LibParse.Json;

using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

public static class JsonReader {
  // `ThreadStatic` due the fields needing to be static in order to be accessed in static functions.
  // Declaring them here prevents the need for them to be passed as parameters to every function.
  [ThreadStatic] private static Stack<List<string>>? _splitArrayPool;
  [ThreadStatic] private static StringBuilder? _stringBuilder;
  [ThreadStatic] private static Dictionary<Type, Dictionary<string, FieldInfo>>? _fieldInfoCache;
  [ThreadStatic] private static Dictionary<Type, Dictionary<string, PropertyInfo>>? _propertyInfoCache;

  /// <summary>
  /// Deserializes a JSON string into an object of type T.
  /// </summary>
  /// <param name="json">The JSON string to be deserialized.</param>
  /// <typeparam name="T">The type of the object to be created. Can be either a class containing public data fields
  ///   or an anonymous `object`.</typeparam>
  /// <returns>An object of type T populated with the data from the JSON string, or the default value of type T if the
  ///   JSON string is empty.</returns>
  /// <remarks>
  /// This method initializes thread-static variables, removes all whitespace characters not within strings from the JSON
  ///   string, and then parses the JSON string into an object of type T.
  /// </remarks>
  public static T? FromJson<T>(this string json) {
    if (json.Length < 1) return default;
    // Initialize, if needed, the ThreadStatic variables
    InitThreadStatic();
    // Remove all whitespace not within strings to make parsing simpler
    json.RemoveWhitespaces();
    // Start parsing the JSON string and cast it into <T>
    return (T)ParseValue(typeof(T), _stringBuilder?.ToString()!)!;
  }

  /// <summary>
  /// Initializes thread-static variables used in the parsing process.
  /// </summary>
  /// <remarks>
  /// This method checks if each of the thread-static variables is null. If a variable is null, it initializes it.
  /// For the StringBuilder variable, it also ensures that it is empty by calling the Clear method.
  /// </remarks>
  private static void InitThreadStatic() {
    _stringBuilder ??= new StringBuilder();
    _splitArrayPool ??= new Stack<List<string>>();
    _fieldInfoCache ??= new Dictionary<Type, Dictionary<string, FieldInfo>>();
    _propertyInfoCache ??= new Dictionary<Type, Dictionary<string, PropertyInfo>>();

    // Make sure `_stringBuilder` is empty
    _stringBuilder.Clear();
  }

  /// <summary>
  /// Appends characters to the StringBuilder until the end of a string is found in the JSON.
  /// </summary>
  /// <param name="appendEscapeCharacter">Whether to append escape characters.</param>
  /// <param name="startIdx">The starting index in the JSON string.</param>
  /// <param name="json">The JSON string.</param>
  /// <returns>The index of the closing quote.</returns>
  private static int AppendUntilStringEnd(bool appendEscapeCharacter, int startIdx, string json) {
    // Write the starting quote to the string builder
    _stringBuilder?.Append(json[startIdx]);
    // Loop through each character in the string and replace double backslashes with singular, stop if a closing quote is found.
    for (var i = startIdx + 1; i < json.Length; i++) {
      switch (json[i]) {
        case '\\':
          if (appendEscapeCharacter) _stringBuilder?.Append(json[i]);
          _stringBuilder?.Append(json[i + 1]);
          // Skip the next backslash as it will be replaced
          i++;
          break;
        case '"':
          _stringBuilder?.Append(json[i]);
          // Return the index of the closing quote
          return i;
        default: _stringBuilder?.Append(json[i]); break;
      }
    }

    return json.Length - 1;
  }

  /// <summary>
  /// Removes all whitespace not within strings to make parsing simpler. Writes the outcome to `_stringBuilder`.
  /// </summary>
  /// <param name="str">The string to remove all whitespaces from.</param>
  private static void RemoveWhitespaces(this string str) {
    for (var i = 0; i < str.Length; i++) {
      var c = str[i];
      if (c == '"') {
        i = AppendUntilStringEnd(true, i, str);
        continue;
      }

      if (char.IsWhiteSpace(c)) continue;

      _stringBuilder?.Append(c);
    }
  }

  /// <summary>
  /// Parses a value from a JSON string into an object of a specified type.
  /// </summary>
  /// <param name="type">The type of the object to be created.</param>
  /// <param name="json">The JSON string representing the object.</param>
  /// <returns>An object of the specified type populated with the data from the JSON string.</returns>
  private static object? ParseValue(Type type, string json) {
    if (json == "null") return null;
    if (type == typeof(string)) return ParseString(json);
    if (type == typeof(decimal)) return ParseDecimal(json);
    if (type == typeof(DateTime)) return ParseDateTime(json);

    if (type.IsEnum) return type.ParseEnum(json);
    if (type.IsArray) return type.ParseArray(json);
    if (type == typeof(object)) return ParseAnonymousValue(json);
    if (json[0] == '{' && json[^1] == '}') return type.ParseObject(json);

    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) return type.ParseList(json);
    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)) return type.ParseDict(json);

    if (type.IsPrimitive) return Convert.ChangeType(json, type, CultureInfo.InvariantCulture);

    return null;
  }

  //Splits { <value>:<value>, <value>:<value> } and [ <value>, <value> ] into a list of <value> strings
  /// <summary>
  /// Splits a JSON string into a list of strings.
  /// </summary>
  /// <param name="json">The JSON string to be split.</param>
  /// <returns>A list of strings obtained by splitting the JSON string.</returns>
  private static List<string> Split(string json) {
    var splitArray = _splitArrayPool?.Count > 0 ? _splitArrayPool.Pop() : [];
    splitArray.Clear();

    // return [] for empty arrays/objects
    if (json.Length == 2) return splitArray;

    _stringBuilder ??= new StringBuilder();

    var parseDepth = 0;
    _stringBuilder.Length = 0;
    for (var i = 1; i < json.Length - 1; i++) {
      switch (json[i]) {
        case '[':
        case '{':
          parseDepth++;
          break;
        case ']':
        case '}':
          parseDepth--;
          break;
        case '"':
          i = AppendUntilStringEnd(true, i, json);
          continue;
        case ',':
        case ':':
          if (parseDepth == 0) {
            splitArray.Add(_stringBuilder.ToString());
            _stringBuilder.Length = 0;
            continue;
          }
          break;
      }

      _stringBuilder.Append(json[i]);
    }

    splitArray.Add(_stringBuilder.ToString());

    return splitArray;
  }


  /// <summary>
  /// Parses a decimal value from a JSON string.
  /// </summary>
  /// <param name="json">The JSON string representing the decimal value.</param>
  /// <returns>The parsed decimal value.</returns>
  private static decimal ParseDecimal(string json) {
    decimal.TryParse(json, NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
    return result;
  }

  /// <summary>
  /// Parses a DateTime value from a JSON string.
  /// </summary>
  /// <param name="json">The JSON string representing the DateTime value.</param>
  /// <returns>The parsed DateTime value.</returns>
  private static DateTime ParseDateTime(string json) {
    DateTime.TryParse(json.Replace("\"", ""), CultureInfo.InvariantCulture, DateTimeStyles.None, out var result);
    return result;
  }

  /// <summary>
  /// Parses an enum value from a JSON string.
  /// </summary>
  /// <param name="type">The type of the enum.</param>
  /// <param name="json">The JSON string representing the enum value.</param>
  /// <returns>The parsed enum value.</returns>
  private static object ParseEnum(this Type type, string json) {
    if (json[0] == '"') json = json.Substring(1, json.Length - 2);
    try {
      return Enum.Parse(type, json, false);
    } catch {
      return 0;
    }
  }

  /// <summary>
  /// Parses a string value from a JSON string.
  /// </summary>
  /// <param name="json">The JSON string representing the string value.</param>
  /// <returns>The parsed string value.</returns>
  private static string ParseString(string json) {
    if (json.Length <= 2) return string.Empty;
    var parseStringBuilder = new StringBuilder(json.Length);
    for (var i = 1; i < json.Length - 1; ++i) {
      if (json[i] == '\\' && i + 1 < json.Length - 1) {
        var j = "\"\\nrtbf/".IndexOf(json[i + 1]);
        if (j >= 0) {
          parseStringBuilder.Append("\"\\\n\r\t\b\f/"[j]);
          ++i;
          continue;
        }
        if (json[i + 1] == 'u' && i + 5 < json.Length - 1) {
          if (int.TryParse(json.AsSpan(i + 2, 4), NumberStyles.AllowHexSpecifier, null, out var c)) {
            parseStringBuilder.Append((char)c);
            i += 5;
            continue;
          }
        }
      }
      parseStringBuilder.Append(json[i]);
    }

    return parseStringBuilder.ToString();
  }

  /// <summary>
  /// Parses an array from a JSON string.
  /// </summary>
  /// <param name="type">The type of the array.</param>
  /// <param name="json">The JSON string representing the array.</param>
  /// <returns>The parsed array.</returns>
  private static Array? ParseArray(this Type type, string json) {
    var arrayType = type.GetElementType();
    if (json[0] != '[' || json[^1] != ']' || arrayType == null) return null;

    var elems = Split(json);
    var newArray = Array.CreateInstance(arrayType, elems.Count);
    for (var i = 0; i < elems.Count; i++) newArray.SetValue(ParseValue(arrayType, elems[i]), i);
    _splitArrayPool?.Push(elems);
    return newArray;
  }

  /// <summary>
  /// Parses a list from a JSON string.
  /// </summary>
  /// <param name="type">The type of the list.</param>
  /// <param name="json">The JSON string representing the list.</param>
  /// <returns>The parsed list.</returns>
  private static IList? ParseList(this Type type, string json) {
    var listType = type.GetGenericArguments()[0];
    if (json[0] != '[' || json[^1] != ']') return null;

    var elems = Split(json);
    var list = (IList?)type.GetConstructor([ typeof(int) ])?.Invoke(new object[] { elems.Count });
    foreach (var elem in elems) list?.Add(ParseValue(listType, elem));
    _splitArrayPool?.Push(elems);
    return list;
  }

  /// <summary>
  /// Parses a dictionary from a JSON string.
  /// </summary>
  /// <param name="type">The type of the dictionary.</param>
  /// <param name="json">The JSON string representing the dictionary.</param>
  /// <returns>The parsed dictionary.</returns>
  private static IDictionary? ParseDict(this Type type, string json) {
    Type keyType, valueType; {
      var args = type.GetGenericArguments();
      keyType = args[0];
      valueType = args[1];
    }

    //Refuse to parse dictionary keys that aren't of type string
    if (keyType != typeof(string)) return null;
    //Must be a valid dictionary element
    if (json[0] != '{' || json[^1] != '}') return null;
    //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
    var elems = Split(json);
    if (elems.Count % 2 != 0) return null;

    var dictionary = (IDictionary?)type.GetConstructor([ typeof(int) ])?.Invoke(new object[] { elems.Count / 2 });
    for (var i = 0; i < elems.Count; i += 2) {
      if (elems[i].Length <= 2) continue;
      var keyValue = elems[i].Substring(1, elems[i].Length - 2);
      var val = ParseValue(valueType, elems[i + 1]);
      dictionary![keyValue] = val;
    }
    return dictionary;
  }

  /// <summary>
  /// Parses an anonymous value from a JSON string.
  /// </summary>
  /// <param name="json">The JSON string representing the anonymous value.</param>
  /// <returns>The parsed anonymous value.</returns>
  private static object? ParseAnonymousValue(string json) {
    if (json.Length == 0) return null;
    if (json == "null") return null;

    if (json[0] == '{' && json[^1] == '}') {
      var elems = Split(json);
      if (elems.Count % 2 != 0) return null;
      var dict = new Dictionary<string, object>(elems.Count / 2);
      for (var i = 0; i < elems.Count; i += 2)
        dict[elems[i].Substring(1, elems[i].Length - 2)] = ParseAnonymousValue(elems[i + 1]);
      return dict;
    }
    if (json[0] == '[' && json[^1] == ']') {
      var items = Split(json);
      var finalList = new List<object>(items.Count);
      foreach (var item in items) finalList.Add(ParseAnonymousValue(item));
      return finalList;
    }
    if (json[0] == '"' && json[^1] == '"') {
      var str = json.Substring(1, json.Length - 2);
      return str.Replace("\\", string.Empty);
    }
    if (char.IsDigit(json[0]) || json[0] == '-') {
      if (json.Contains('.')) {
        double.TryParse(json, NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
        return result;
      } else {
        int.TryParse(json, out var result);
        return result;
      }
    }

    return json switch {
      "true"  => true,
      "false" => false,
      _       => null
    };
  }

  /// <summary>
  /// Creates a dictionary mapping member names to members.
  /// </summary>
  /// <param name="members">The members to be included in the dictionary.</param>
  /// <returns>A dictionary mapping member names to members.</returns>
  private static Dictionary<string, T> CreateMemberNameDictionary<T>(IEnumerable<T> members) where T : MemberInfo {
    var nameToMember = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
    foreach (var member in members) {
      if (member.IsDefined(typeof(IgnoreDataMemberAttribute), true)) continue;

      var name = member.Name;
      if (member.IsDefined(typeof(DataMemberAttribute), true)) {
        var dataMemberAttribute = (DataMemberAttribute?)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true);
        if (!string.IsNullOrEmpty(dataMemberAttribute?.Name)) name = dataMemberAttribute.Name;
      }

      nameToMember.Add(name, member);
    }

    return nameToMember;
  }

  /// <summary>
  /// Parses a JSON object into an instance of a specified type.
  /// </summary>
  /// <param name="type">The type of the object to be created.</param>
  /// <param name="json">The JSON string representing the object.</param>
  /// <returns>An object of the specified type populated with the data from the JSON string.</returns>
  /// <remarks>
  /// This method uses reflection to create an instance of the specified type and populate its public fields and properties
  /// with the data from the JSON string. It supports complex types with nested objects and arrays.
  /// If a field or property is marked with the IgnoreDataMemberAttribute, it will be ignored.
  /// If a field or property is marked with the DataMemberAttribute, the name specified in the attribute will be used as the key in the JSON object.
  /// </remarks>
  private static object ParseObject(this Type type, string json) {
    var instance = type.GetConstructor(Type.EmptyTypes)?.Invoke(null);
    if (instance == null) return type;

    //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
    var elems = Split(json);
    if (elems.Count % 2 != 0) return instance;

    if (!_fieldInfoCache!.TryGetValue(type, out var nameToField)) {
      nameToField = CreateMemberNameDictionary(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));
      _fieldInfoCache.Add(type, nameToField);
    }
    if (!_propertyInfoCache!.TryGetValue(type, out var nameToProperty)) {
      nameToProperty = CreateMemberNameDictionary(type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));
      _propertyInfoCache.Add(type, nameToProperty);
    }

    for (var i = 0; i < elems.Count; i += 2) {
      if (elems[i].Length <= 2) continue;
      var key = elems[i].Substring(1, elems[i].Length - 2);
      var value = elems[i + 1];

      if (nameToField.TryGetValue(key, out var fieldInfo))
        fieldInfo.SetValue(instance, ParseValue(fieldInfo.FieldType, value));
      else if (nameToProperty.TryGetValue(key, out var propertyInfo))
        propertyInfo.SetValue(instance, ParseValue(propertyInfo.PropertyType, value), null);
    }

    return instance;
  }
}
