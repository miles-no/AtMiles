package no.miles.atmiles.employee;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class EmployeeDummyContent {

    public static List<EmployeeItem> ITEMS = new ArrayList<EmployeeItem>();

    /**
     * A map of sample (dummy) items, by ID.
     */
    public static Map<String, EmployeeItem> ITEM_MAP = new HashMap<String, EmployeeItem>();

    static {
        // Add 3 sample items.
        addItem(new EmployeeItem("1", "Ole Olsen"));
        addItem(new EmployeeItem("2", "Jens Jensen"));
        addItem(new EmployeeItem("3", "Per Persen"));
        addItem(new EmployeeItem("4", "Lars Larsen"));
    }

    private static void addItem(EmployeeItem item) {
        ITEMS.add(item);
        ITEM_MAP.put(item.id, item);
    }
}
