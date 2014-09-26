package no.miles.atmiles.employee;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Created by royveshovda on 26/09/14.
 */
public class EmployeeDummyContent {

    public static List<EmployeeItem> ITEMS = new ArrayList<EmployeeItem>();

    /**
     * A map of sample (dummy) items, by ID.
     */
    public static Map<String, EmployeeItem> ITEM_MAP = new HashMap<String, EmployeeItem>();

    static {
        // Add 3 sample items.
        addItem(new EmployeeItem("1", "Item 1"));
        addItem(new EmployeeItem("2", "Item 2"));
        addItem(new EmployeeItem("3", "Item 3"));
    }

    private static void addItem(EmployeeItem item) {
        ITEMS.add(item);
        ITEM_MAP.put(item.id, item);
    }
}
