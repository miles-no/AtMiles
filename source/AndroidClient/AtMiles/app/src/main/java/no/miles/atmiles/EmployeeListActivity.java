package no.miles.atmiles;

import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.app.Activity;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.TextView;
import android.widget.Toast;

import org.json.JSONException;
import org.json.JSONObject;


/**
 * An activity representing a list of Employees. This activity
 * has different presentations for handset and tablet-size devices. On
 * handsets, the activity presents a list of items, which when touched,
 * lead to a {@link EmployeeDetailActivity} representing
 * item details. On tablets, the activity presents the list of items and
 * item details side-by-side using two vertical panes.
 * <p>
 * The activity makes heavy use of fragments. The list of items is a
 * {@link EmployeeListFragment} and the item details
 * (if present) is a {@link EmployeeDetailFragment}.
 * <p>
 * This activity also implements the required
 * {@link EmployeeListFragment.Callbacks} interface
 * to listen for item selections.
 */
public class EmployeeListActivity extends Activity
        implements EmployeeListFragment.Callbacks {

    static final String ClientID = "6jsWdVCPDiKSdSKi2n7naqmy7eeO703H";
    static final String Tenant = "atmiles";
    static final String Callback = "oob://atmiles/android";
    static final String Connection = "2";


    /**
     * Whether or not the activity is in two-pane mode, i.e. running on a tablet
     * device.
     */
    private boolean mTwoPane;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_employee_list);

        if (findViewById(R.id.employee_detail_container) != null) {
            // The detail container view will be present only in the
            // large-screen layouts (res/values-large and
            // res/values-sw600dp). If this view is present, then the
            // activity should be in two-pane mode.
            mTwoPane = true;

            // In two-pane mode, list items should be given the
            // 'activated' state when touched.
            ((EmployeeListFragment) getFragmentManager()
                    .findFragmentById(R.id.employee_list))
                    .setActivateOnItemClick(true);
        }

        // TODO: If exposing deep links into your app, handle intents here.
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.home, menu);
        return true;
    }

    /**
     * Callback method from {@link EmployeeListFragment.Callbacks}
     * indicating that the item with the given ID was selected.
     */
    @Override
    public void onItemSelected(String id) {
        if (mTwoPane) {
            // In two-pane mode, show the detail view in this activity by
            // adding or replacing the detail fragment using a
            // fragment transaction.
            Bundle arguments = new Bundle();
            arguments.putString(EmployeeDetailFragment.ARG_ITEM_ID, id);
            EmployeeDetailFragment fragment = new EmployeeDetailFragment();
            fragment.setArguments(arguments);
            getFragmentManager().beginTransaction()
                    .replace(R.id.employee_detail_container, fragment)
                    .commit();

        } else {
            // In single-pane mode, simply start the detail activity
            // for the selected item ID.
            Intent detailIntent = new Intent(this, EmployeeDetailActivity.class);
            detailIntent.putExtra(EmployeeDetailFragment.ARG_ITEM_ID, id);
            startActivity(detailIntent);
        }
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        boolean handled = true;
        int id = item.getItemId();

        switch(id){
            case R.id.action_menu_login:
                onMenuClickLogin(item);
                break;
            case R.id.action_menu_favorites:
                startActivity(new Intent(this, FavoritesActivity.class));
                break;
            case R.id.action_menu_profile:
                startActivity(new Intent(this, ProfileActivity.class));
                break;
            case R.id.action_menu_exit:
                finish();
                break;
            default:
                handled = super.onOptionsItemSelected(item);
        }
        return handled;
    }

    private void onMenuClickLogin(MenuItem item) {
        Intent authActivity = new Intent(EmployeeListActivity.this,
                no.miles.atmiles.AuthenticationActivity.class);

        AuthenticationActivitySetup setup;
        setup = new AuthenticationActivitySetup(Tenant, ClientID, Callback);

        authActivity.putExtra(AuthenticationActivity.AUTHENTICATION_SETUP, setup);

        startActivityForResult(authActivity, AuthenticationActivity.AUTH_REQUEST_COMPLETE);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent authActivityResult) {
        super.onActivityResult(requestCode, resultCode, authActivityResult);

        switch(requestCode)
        {
            case AuthenticationActivity.AUTH_REQUEST_COMPLETE:
                if(resultCode==RESULT_OK)
                {
                    AuthenticationActivityResult result;
                    result = (AuthenticationActivityResult) authActivityResult.getSerializableExtra(AuthenticationActivity.AUTHENTICATION_RESULT);

                    String jsonWebToken = result.JsonWebToken;
                    String accessToken = result.accessToken;

                    //String userInfoUrl = String.format("https://api.auth0.com/userinfo?access_token=%s", result.accessToken);
                    String securedUrl = "http://192.168.77.100/secured/ping";
                    new AsyncTask<String, Void, JSONObject>() {
                        @Override
                        protected JSONObject doInBackground(String... url) {
                            JSONObject json = RestJsonClient.connect(url[0]);
                            return json;
                        }

                        @Override
                        protected void onPostExecute(JSONObject user) {

                            try {
                                Toast.makeText(EmployeeListActivity.this, user.toString(2), Toast.LENGTH_LONG).show();
                                //((TextView) findViewById(R.id.user)).setText(user.toString(2));
                            } catch (JSONException e) {
                                e.printStackTrace();
                            }
                        }
                    }.execute(securedUrl);

                }
                break;
        }
    }
}
