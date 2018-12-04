package com.applitools.ImageTester;

import com.sun.xml.internal.ws.util.StringUtils;
import org.apache.commons.cli.CommandLine;
import org.apache.commons.cli.ParseException;

import java.util.EnumSet;

public class Utils {

    public static <T extends Enum<T>> T parseEnum(Class<T> c, String string) throws ParseException {
        if (c != null && string != null) {
            try {
                return Enum.valueOf(c, string.trim().toUpperCase());
            } catch (IllegalArgumentException ex) {
            }
        }
        throw new ParseException(String.format("Unable to parse value %s for enum %s", string, c.getName()));
    }

    public static String getEnumValues(Class type) {
        StringBuilder sb = new StringBuilder();
        for (Object val : EnumSet.allOf(type)) {
            sb.append(StringUtils.capitalize(val.toString().toLowerCase()));
            sb.append('|');
        }
        return sb.substring(0, sb.length() - 1);
    }
}
