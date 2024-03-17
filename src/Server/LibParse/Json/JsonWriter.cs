namespace LibParse.Json;

using System.Reflection;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

public static class JsonWriter {
  /// <summary>
  /// Converts an object into a JSON string.
  /// </summary>
  /// <param name="item">The object to be converted into a JSON string.</param>
  /// <returns>A JSON string representation of the object.</returns>
  public static string ToJson(this object item) {
    // A `StringWriter` is used instead of just `StringBuilder` due to its higher efficiency with smaller strings.
    var writer = new StringWriter();

    // Start the recursive call to append the values of `item` to the writer.
    AppendValue(writer, item);

    // Return the string representation of `item` as JSON object.
    return writer.ToString();
  }

  /// <summary>
  ///  Appends the value of `item` to the writer.
  ///  If type = any decimal numeric type, boolean or DateTime, append the value as is.
  ///   If type = any other type, append the value as is.
  ///  If type = string or char, append the value as "string", escaping special characters.
  ///  If type = object, check if it's an enum, list, or dictionary, go as deep as the nested object goes and append the value:
  ///   - If it's an enum, append the value as "enum"
  ///   - If it's a list, append the value as "[item1,item2,...]"
  ///   - If it's a dictionary, append the value as "{\"key1\":value1,\"key2\":value2,...}"
  ///   - For any other object type (class, ...), loop through each field and property in the object and append the value.
  /// </summary>
  /// <param name="writer">The `TextWriter` to write the variable into</param>
  /// <param name="item">The item to cast into `writer`</param>
  private static void AppendValue(TextWriter writer, object item) {
    // Get the type of `item`. This is used to determine how to append the value to the writer.
    var type = item.GetType();
    switch (Type.GetTypeCode(type)) {
      case TypeCode.String:
      case TypeCode.Char:
        writer.Write('"');
        // Loop through each character in the string and escape special characters.
        foreach (var c in item.ToString()!) writer.Write(EscapeCharacter(c));
        writer.Write('"');
        break;
      case TypeCode.Object: item.CastIntoWriter(writer, type); break;
      case TypeCode.Single: writer.Write(((float)item).ToString(CultureInfo.InvariantCulture)); break;
      case TypeCode.Double: writer.Write(((double)item).ToString(CultureInfo.InvariantCulture)); break;
      case TypeCode.Decimal: writer.Write(((decimal)item).ToString(CultureInfo.InvariantCulture)); break;
      case TypeCode.Boolean: writer.Write((bool)item ? "true" : "false"); break;
      case TypeCode.DateTime: writer.Write($"\"{((DateTime)item).ToString(CultureInfo.InvariantCulture)}\""); break;
      default: writer.Write(item); break;
    }
  }


  /// <summary>
  /// Escapes special characters in a string.
  /// </summary>
  /// <param name="c">The character to be escaped.</param>
  /// <returns>The escaped character as a string.</returns>
  private static string EscapeCharacter(char c) {
    if (c is not (< ' ' or '"' or '\\')) return c.ToString();

    var j = "\"\\nrtbf/".IndexOf(c);
    return j > -1 ? "\"\\\n\r\t\b\f/"[j].ToString() : $"\\u{(int)c:X4}";
  }

  /// <summary>
  /// Casts an object into a TextWriter.
  /// </summary>
  /// <param name="item">The object to be cast.</param>
  /// <param name="writer">The TextWriter to cast the object into.</param>
  /// <param name="type">The type of the object.</param>
  private static void CastIntoWriter(this object item, TextWriter writer, Type type) {
    if (type.IsEnum) writer.Write($"\"{item}\"");
    else if (item is IList list) list.ListToString(writer);
    else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
      (item as IDictionary)!.DictionaryToString(writer, type);
    else item.ClassToString(writer, type);
  }

  /// <summary>
  /// Converts a list into a string and appends it to a TextWriter.
  /// </summary>
  /// <param name="list">The list to be converted into a string.</param>
  /// <param name="writer">The TextWriter to append the string to.</param>
  private static void ListToString(this IEnumerable list, TextWriter writer) {
    var isFirst = true;
    writer.Write('[');
    // Loop through each item in the list and append the value.
    foreach (var e in list)  {
      if (isFirst) isFirst = false;
      else writer.Write(',');
      AppendValue(writer, e);
    }
    writer.Write(']');
  }

  /// <summary>
  /// Converts a dictionary into a string and appends it to a TextWriter.
  /// </summary>
  /// <param name="dict">The dictionary to be converted into a string.</param>
  /// <param name="writer">The TextWriter to append the string to.</param>
  /// <param name="type">The type of the dictionary.</param>
  private static void DictionaryToString(this IDictionary dict, TextWriter writer, Type type) {
    // Get type of the dictionary key. Refuse to output dictionary keys that aren't of type string
    var keyType = type.GetGenericArguments()[0];
    if (keyType != typeof(string)) {
      writer.Write("{}");
      return;
    }

    var isFirst = true;
    writer.Write('{');
    // Loop through each key-value pair in the dictionary and append the value.
    foreach (var key in dict.Keys) {
      if (isFirst) isFirst = false;
      else writer.Write(',');

      writer.Write($"\"{(string)key}\":");
      // Recursive call to append the value of the dictionary key to the writer. Will go as deep as the nested object goes.
      AppendValue(writer, dict[key]!);
    }
    writer.Write('}');
  }

  /// <summary>
  /// Gets the name of a member.
  /// </summary>
  /// <param name="member">The member whose name is to be gotten.</param>
  /// <returns>The name of the member.</returns>
  private static string GetMemberName(MemberInfo member) {
    if (member.IsDefined(typeof(DataMemberAttribute), true)) return member.Name;

    var dataMemberAttribute = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true)!;

    return !string.IsNullOrEmpty(dataMemberAttribute?.Name) ? dataMemberAttribute.Name : member.Name;
  }

  /// <summary>
  /// Converts an object into a string and appends it to a TextWriter.
  /// </summary>
  /// <param name="item">The object to be converted into a string.</param>
  /// <param name="writer">The TextWriter to append the string to.</param>
  /// <param name="type">The type of the object.</param>
  private static void ClassToString(this object item, TextWriter writer, Type type) {
    var isFirst = true;
    var fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

    writer.Write('{');
    foreach (var field in fieldInfos) {
      if (field.IsDefined(typeof(IgnoreDataMemberAttribute), true)) continue;

      var value = field.GetValue(item);
      if (value == null) continue;

      if (isFirst) isFirst = false;
      else writer.Write(',');

      writer.Write($"\"{GetMemberName(field)}\":");
      AppendValue(writer, value);
    }

    var propertyInfo = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
    foreach (var property in propertyInfo) {
      // Ignore private fields
      if (!property.CanRead || property.IsDefined(typeof(IgnoreDataMemberAttribute), true)) continue;

      var value = property.GetValue(item, null);
      if (value == null) continue;

      if (isFirst) isFirst = false;
      else writer.Write(',');

      writer.Write($"\"{GetMemberName(property)}\":");
      AppendValue(writer, value);
    }
    writer.Write('}');
  }
}
