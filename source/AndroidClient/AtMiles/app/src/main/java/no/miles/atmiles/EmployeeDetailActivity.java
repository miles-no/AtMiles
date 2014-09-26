package no.miles.atmiles;

import android.content.Intent;
import android.os.Bundle;
import android.app.Activity;

import android.view.Menu;
import android.view.MenuItem;
import android.widget.Toast;


/**
 * An activity representing a single Employee detail screen. This
 * activity is only used on handset devices. On tablet-size devices,
 * item details are presented side-by-side with a list of items
 * in a {@link EmployeeListActivity}.
 * <p>
 * This activity is mostly just a 'shell' activity containing nothing
 * more than a {@link EmployeeDetailFragment}.
 */
public class EmployeeDetailActivity extends Activity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_employee_detail);

        // Show the Up button in the action bar.
        getActionBar().setDisplayHomeAsUpEnabled(true);

        // savedInstanceState is non-null when there is fragment state
        // saved from previous configurations of this activity
        // (e.g. when rotating the screen from portrait to landscape).
        // In this case, the fragment will automatically be re-added
        // to its container so we don't need to manually add it.
        // For more information, see the Fragments API guide at:
        //
        // http://developer.android.com/guide/components/fragments.html
        //
        if (savedInstanceState == null) {
            // Create the detail fragment and add it to the activity
            // using a fragment transaction.
            Bundle arguments = new Bundle();
            arguments.putString(EmployeeDetailFragment.ARG_ITEM_ID,
                    getIntent().getStringExtra(EmployeeDetailFragment.ARG_ITEM_ID));
            EmployeeDetailFragment fragment = new EmployeeDetailFragment();
            fragment.setArguments(arguments);
            getFragmentManager().beginTransaction()
                    .add(R.id.employee_detail_container, fragment)
                    .commit();
        }
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.employee, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        boolean handled = true;
        int id = item.getItemId();

        switch(id){
            case android.R.id.home:
                navigateUpTo(new Intent(this, EmployeeListActivity.class));
                break;
            case R.id.action_menu_favorites:
                startActivity(new Intent(this, FavoritesActivity.class));
                break;
            case R.id.action_menu_profile:
                startActivity(new Intent(this, ProfileActivity.class));
                break;
            default:
                handled = super.onOptionsItemSelected(item);
        }
        return handled;
    }
}
