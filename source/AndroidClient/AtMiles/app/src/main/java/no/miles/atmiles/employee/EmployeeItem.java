package no.miles.atmiles.employee;

/**
 * Created by Roy on 26.09.2014.
 */
public class EmployeeItem {
    public String id;
    public String content;

    public EmployeeItem(String id, String content) {
        this.id = id;
        this.content = content;
    }

    @Override
    public String toString() {
        return content;
    }
}
